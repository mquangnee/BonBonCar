using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQueries
{
    /// <summary>
    /// Search available cars by location and rental time window.
    /// </summary>
    public class SearchCarsQuery : IRequest<MethodResult<IList<RentalCarModel>>>
    {
        public string? Location { get; set; }
        public DateTime? PickupDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchCarsQueryHandler : IRequestHandler<SearchCarsQuery, MethodResult<IList<RentalCarModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<RentalCarModel>>> Handle(SearchCarsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<RentalCarModel>>();

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

            if (request.Page <= 0) request.Page = 1;
            if (request.PageSize <= 0) request.PageSize = 10;

            // Any order that overlaps the requested window will block the car.
            // Overlap condition: order.Start < requestedReturn AND order.End > requestedPickup
            var conflictingVehicleIdsQuery = _unitOfWork.RentalOrders
                .QueryableAsync()
                .Where(o =>
                    o.Status != RentalOrderStatus.Cancelled &&
                    o.StartDate < ret &&
                    o.EndDate > pickup)
                .Select(o => o.CarId)
                .Distinct();

            IQueryable<Car> carsQuery = _unitOfWork.Cars
                .QueryableAsync()
                .Where(c => c.Status == EnumCarStatus.Available)
                .Where(c => !conflictingVehicleIdsQuery.Contains(c.Id));

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var location = request.Location.Trim();
                carsQuery = carsQuery.Where(c => c.PickupAddress != null && EF.Functions.Like(c.PickupAddress, $"%{location}%"));
            }

            var cars = await carsQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (cars == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var carIds = cars.Select(c => c.Id).ToList();
            var modelIds = cars.Select(c => c.ModelId).Distinct().ToList();

            var models = await _unitOfWork.Models
                .QueryableAsync()
                .Where(m => modelIds.Contains(m.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var brandIds = models.Select(m => m.BrandId).Distinct().ToList();
            var brands = await _unitOfWork.Brands
                .QueryableAsync()
                .Where(b => brandIds.Contains(b.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var images = await _unitOfWork.CarImages
                .QueryableAsync()
                .Where(i => carIds.Contains(i.CarId))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var prices = await _unitOfWork.CarPrices
                .QueryableAsync()
                .Where(p => carIds.Contains(p.CarId))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = cars.Select(car =>
            {
                var model = models.FirstOrDefault(m => m.Id == car.ModelId);
                var brand = model == null ? null : brands.FirstOrDefault(b => b.Id == model.BrandId);

                var carImages = images.Where(i => i.CarId == car.Id).ToList();
                var thumbnail = carImages.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? carImages.FirstOrDefault()?.ImageUrl;

                var carPrices = prices
                    .Where(p => p.CarId == car.Id)
                    .OrderBy(p => p.RentalDuration)
                    .Select(p => new CarPriceModel
                    {
                        RentalDuration = p.RentalDuration,
                        Price = p.Price
                    })
                    .ToList();

                return new RentalCarModel
                {
                    BrandName = brand?.Name,
                    ModelName = model?.Name,
                    Year = car.Year,
                    LicensePlate = car.LicensePlate,
                    ThumbnailUrl = thumbnail,
                    Features = car.Features?.ToList(),
                    Images = carImages.Select(i => i.ImageUrl ?? string.Empty).Where(u => !string.IsNullOrWhiteSpace(u)).ToList(),
                    Prices = carPrices,
                    Status = car.Status
                };
            }).ToList();

            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

