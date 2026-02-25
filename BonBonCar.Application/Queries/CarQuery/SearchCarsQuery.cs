using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.Enums.RentalOrder;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQuery
{
    public class SearchCarsQuery : IRequest<MethodResult<IList<CarModel>>>
    {
        public string? Location { get; set; }
        public DateTime? PickupDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchCarsQueryHandler : IRequestHandler<SearchCarsQuery, MethodResult<IList<CarModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<CarModel>>> Handle(SearchCarsQuery request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);

            var methodResult = new MethodResult<IList<CarModel>>();

            if (!request.PickupDateTime.HasValue || !request.ReturnDateTime.HasValue)
            {
                methodResult.AddErrorBadRequest("pickupDateTime and returnDateTime are required.");
                return methodResult;
            }

            var pickup = request.PickupDateTime.Value;
            var ret = request.ReturnDateTime.Value;

            if (pickup >= ret)
            {
                methodResult.AddErrorBadRequest("pickupDateTime must be earlier than returnDateTime.");
                return methodResult;
            }

            var now = DateTime.Now;
            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 50);

            var carsQuery = _unitOfWork.Cars.QueryableAsync()
                .AsNoTracking()
                .Where(c => c.Status == EnumCarStatus.Available);

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var locationRaw = request.Location.Trim();
                carsQuery = carsQuery.Where(c =>
                    (c.Location != null && EF.Functions.Like(c.Location, $"%{locationRaw}%")) ||
                    (c.PickupAddress != null && EF.Functions.Like(c.PickupAddress, $"%{locationRaw}%")));
            }

            var rentalOrders = _unitOfWork.RentalOrders.QueryableAsync().AsNoTracking();
            var payments = _unitOfWork.Payments.QueryableAsync().AsNoTracking();

            carsQuery = carsQuery.Where(c =>
                !rentalOrders.Any(o =>
                    o.CarId == c.Id
                    && o.PickupDateTime < ret
                    && o.ReturnDateTime > pickup
                    && (
                        o.Status == EnumRentalOrderStatus.Held
                        || (
                            o.Status == EnumRentalOrderStatus.HoldPending
                            && payments.Any(p =>
                                p.RentalOrderId == o.Id
                                && p.ExpiresAt > now
                                && p.Status != EnumPaymentStatus.Failed
                                && p.Status != EnumPaymentStatus.Expired
                            )
                        )
                    )
                )
            );

            var cars = await carsQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            if (cars.Count == 0)
            {
                methodResult.Result = new List<CarModel>();
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            var carIds = cars.Select(c => c.Id).ToList();
            var modelIds = cars.Select(c => c.ModelId).Distinct().ToList();

            var models = await _unitOfWork.Models.QueryableAsync().AsNoTracking()
                .Where(m => modelIds.Contains(m.Id))
                .ToListAsync(ct);

            var brandIds = models.Select(m => m.BrandId).Distinct().ToList();
            var brands = await _unitOfWork.Brands.QueryableAsync().AsNoTracking()
                .Where(b => brandIds.Contains(b.Id))
                .ToListAsync(ct);

            var images = await _unitOfWork.CarImages.QueryableAsync().AsNoTracking()
                .Where(i => carIds.Contains(i.CarId))
                .ToListAsync(ct);

            var prices = await _unitOfWork.CarPrices.QueryableAsync().AsNoTracking()
                .Where(p => carIds.Contains(p.CarId))
                .ToListAsync(ct);

            var modelById = models.ToDictionary(x => x.Id, x => x);
            var brandById = brands.ToDictionary(x => x.Id, x => x);

            var imagesByCar = images
                .Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl))
                .GroupBy(i => i.CarId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pricesByCar = prices
                .GroupBy(p => p.CarId)
                .ToDictionary(g => g.Key, g => g.ToList());

            decimal GetPrice(Guid carId, EnumRentalDuration duration)
            {
                if (!pricesByCar.TryGetValue(carId, out var list) || list == null) return 0m;
                return list.FirstOrDefault(x => x.RentalDuration == duration)?.Price ?? 0m;
            }

            string? GetPrimaryImage(Guid carId)
            {
                if (!imagesByCar.TryGetValue(carId, out var list) || list == null || list.Count == 0) return null;

                return list
                    .OrderByDescending(x => x.IsPrimary)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault();
            }

            var result = cars.Select(car =>
            {
                modelById.TryGetValue(car.ModelId, out var model);
                var brand = model != null && brandById.TryGetValue(model.BrandId, out var b) ? b : null;

                var brandName = brand?.Name?.Trim();
                var modelName = model?.Name?.Trim();

                var carName = string.Join(" ",
                    new[] { brandName, modelName, car.Year.ToString() }
                        .Where(x => !string.IsNullOrWhiteSpace(x)));

                return new CarModel
                {
                    CarId = car.Id,
                    CarName = string.IsNullOrWhiteSpace(carName) ? $"Xe {car.Year}" : carName,
                    PickupAddress = car.PickupAddress,
                    PrimaryImageUrl = GetPrimaryImage(car.Id),
                    Price4h = GetPrice(car.Id, EnumRentalDuration.hour4),
                    Price24h = GetPrice(car.Id, EnumRentalDuration.hour24),
                };
            }).ToList();

            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}