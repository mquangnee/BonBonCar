using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQuery
{
    public class GetCarDetailForRentQuery : IRequest<MethodResult<CarForRentModel>>
    {
        public Guid CarId { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }

    }
    public class GetCarDetailForRentQueryHandler : IRequestHandler<GetCarDetailForRentQuery, MethodResult<CarForRentModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarDetailForRentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<CarForRentModel>> Handle(GetCarDetailForRentQuery request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CarForRentModel>();
            if (request.CarId == Guid.Empty)
            {
                methodResult.AddErrorBadRequest("carId is required.");
                return methodResult;
            }
            if (request.PickupDateTime >= request.ReturnDateTime)
            {
                methodResult.AddErrorBadRequest("pickupDateTime must be earlier than returnDateTime.");
                return methodResult;
            }
            var car = await _unitOfWork.Cars.QueryableAsync().FirstOrDefaultAsync(x => x.Id == request.CarId, ct);

            if (car == null)
            {
                methodResult.AddErrorBadRequest("Car not found.");
                return methodResult;
            }
            var model = await _unitOfWork.Models.QueryableAsync().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == car.ModelId, ct);

            var brand = model == null
                ? null
                : await _unitOfWork.Brands.QueryableAsync().AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == model.BrandId, ct);

            var carName = string.Join(" ",
                new[]
                {
                    brand?.Name?.Trim(),
                    model?.Name?.Trim(),
                    car.Year.ToString()
                }.Where(x => !string.IsNullOrWhiteSpace(x)));

            if (string.IsNullOrWhiteSpace(carName))
                carName = $"Xe {car.Year}";

            var images = await _unitOfWork.CarImages.QueryableAsync().AsNoTracking()
                .Where(i => i.CarId == car.Id && i.ImageUrl != null && i.ImageUrl != "")
                .OrderByDescending(i => i.IsPrimary)
                .ToListAsync(ct);

            var primaryImage = images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                               ?? images.FirstOrDefault()?.ImageUrl;

            var priceRows = await _unitOfWork.CarPrices.QueryableAsync().AsNoTracking()
                .Where(p => p.CarId == car.Id)
                .ToListAsync(ct);

            decimal GetPrice(EnumRentalDuration d)
                => priceRows.FirstOrDefault(x => x.RentalDuration == d)?.Price ?? 0m;

            var p4 = GetPrice(EnumRentalDuration.hour4);
            var p8 = GetPrice(EnumRentalDuration.hour8);
            var p12 = GetPrice(EnumRentalDuration.hour12);
            var p24 = GetPrice(EnumRentalDuration.hour24);

            var totalHours = CalcTotalHoursCeil(request.PickupDateTime, request.ReturnDateTime);
            var (totalPrice, breakdown) = CalcPrice(totalHours, p4, p8, p12, p24);

            methodResult.Result = new CarForRentModel
            {
                CarId = car.Id,
                CarName = carName,
                BrandName = brand?.Name,
                ModelName = model?.Name,
                Year = car.Year,
                Location = car.Location,
                PickupAddress = car.PickupAddress?.Trim(),
                PickupDateTime = request.PickupDateTime,
                ReturnDateTime = request.ReturnDateTime,
                PrimaryImageUrl = primaryImage,
                Images = images.Select(x => x.ImageUrl!).ToList(),
                Features = car.Features?.ToList() ?? new List<string>(),
                Price4h = p4,
                Price8h = p8,
                Price12h = p12,
                Price24h = p24,
                TotalHours = totalHours,
                TotalPrice = totalPrice,
                Breakdown = breakdown
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }

        private static int CalcTotalHoursCeil(DateTime pickup, DateTime ret)
        {
            var minutes = (ret - pickup).TotalMinutes;
            if (minutes <= 0) return 0;
            return (int)Math.Ceiling(minutes / 60d);
        }

        private static (decimal total, IList<PriceBreakdownItem> breakdown) CalcPrice(
            int totalHours, decimal p4, decimal p8, decimal p12, decimal p24)
        {
            var breakdown = new List<PriceBreakdownItem>();
            if (totalHours <= 0) return (0m, breakdown);

            void Add(int packageHours, int qty, decimal unit)
            {
                if (qty <= 0) return;
                breakdown.Add(new PriceBreakdownItem
                {
                    PackageHours = packageHours,
                    Quantity = qty,
                    UnitPrice = unit,
                    SubTotal = qty * unit
                });
            }

            var days = totalHours / 24;
            var rem = totalHours % 24;

            if (days > 0) Add(24, days, p24);
            if (rem > 0)
            {
                if (rem <= 4) Add(4, 1, p4);
                else if (rem <= 8) Add(8, 1, p8);
                else if (rem <= 12) Add(12, 1, p12);
                else Add(24, 1, p24);
            }
            var total = breakdown.Sum(x => x.SubTotal);
            return (total, breakdown);
        }
    }
}
