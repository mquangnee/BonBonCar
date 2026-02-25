using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.QueryModels.RentalOrders;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.Rentals
{
    public class GetMyActiveRentalOrdersQuery : IRequest<MethodResult<MyActiveRentalsResult>>
    {
        public Guid CustomerId { get; set; }
    }

    public class GetMyActiveRentalOrdersQueryHandler : IRequestHandler<GetMyActiveRentalOrdersQuery, MethodResult<MyActiveRentalsResult>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyActiveRentalOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<MyActiveRentalsResult>> Handle(GetMyActiveRentalOrdersQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<MyActiveRentalsResult>();

            if (request.CustomerId == Guid.Empty)
            {
                methodResult.AddErrorBadRequest("Không xác định được người dùng!");
                return methodResult;
            }

            var orders = await _unitOfWork.RentalOrders
                .GetMyActiveAsync(request.CustomerId, cancellationToken);

            var orderIds = orders.Select(o => o.Id).ToList();

            var carIds = orders.Select(o => o.CarId).Distinct().ToList();

            var cars = await _unitOfWork.Cars.QueryableAsync()
                .AsNoTracking()
                .Where(c => carIds.Contains(c.Id))
                .Select(c => new { c.Id, c.ModelId, c.Year })
                .ToListAsync(cancellationToken);

            var modelIds = cars.Select(c => c.ModelId).Distinct().ToList();

            var models = await _unitOfWork.Models.QueryableAsync()
                .AsNoTracking()
                .Where(m => modelIds.Contains(m.Id))
                .Select(m => new { m.Id, m.Name, m.BrandId })
                .ToListAsync(cancellationToken);

            var brandIds = models.Select(m => m.BrandId).Distinct().ToList();

            var brands = await _unitOfWork.Brands.QueryableAsync()
                .AsNoTracking()
                .Where(b => brandIds.Contains(b.Id))
                .Select(b => new { b.Id, b.Name })
                .ToListAsync(cancellationToken);

            var images = await _unitOfWork.CarImages.QueryableAsync()
                .AsNoTracking()
                .Where(i => carIds.Contains(i.CarId) && !string.IsNullOrWhiteSpace(i.ImageUrl))
                .Select(i => new { i.CarId, i.ImageUrl, i.IsPrimary })
                .ToListAsync(cancellationToken);

            var carById = cars.ToDictionary(x => x.Id, x => x);
            var modelById = models.ToDictionary(x => x.Id, x => x);
            var brandById = brands.ToDictionary(x => x.Id, x => x);

            var imagesByCar = images
                .GroupBy(i => i.CarId)
                .ToDictionary(g => g.Key, g => g.ToList());

            string GetCarName(Guid carId)
            {
                if (!carById.TryGetValue(carId, out var car))
                {
                    return string.Empty;
                }

                modelById.TryGetValue(car.ModelId, out var model);
                var brand = model != null && brandById.TryGetValue(model.BrandId, out var b) ? b : null;

                var brandName = brand?.Name?.Trim();
                var modelName = model?.Name?.Trim();

                var name = string.Join(" ",
                    new[] { brandName, modelName, car.Year.ToString() }
                        .Where(x => !string.IsNullOrWhiteSpace(x)));

                return string.IsNullOrWhiteSpace(name) ? $"Xe {car.Year}" : name;
            }

            string? GetPrimaryImage(Guid carId)
            {
                if (!imagesByCar.TryGetValue(carId, out var list) || list == null || list.Count == 0)
                {
                    return null;
                }

                return list
                    .OrderByDescending(x => x.IsPrimary)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault();
            }

            var ordersMissingTotal = orders
                .Where(o => o.TotalRentalFee <= 0m)
                .ToList();

            Dictionary<Guid, decimal> computedTotalByOrderId = new();
            if (ordersMissingTotal.Count > 0)
            {
                var carIdsForMissingTotal = ordersMissingTotal.Select(o => o.CarId).Distinct().ToList();

                var carPrices = await _unitOfWork.CarPrices.QueryableAsync()
                    .AsNoTracking()
                    .Where(p => carIdsForMissingTotal.Contains(p.CarId))
                    .Select(p => new { p.CarId, p.RentalDuration, p.Price })
                    .ToListAsync(cancellationToken);

                var pricesByCarId = carPrices
                    .GroupBy(x => x.CarId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var o in ordersMissingTotal)
                {
                    if (!pricesByCarId.TryGetValue(o.CarId, out var pricesForCar))
                    {
                        continue;
                    }

                    var p4 = pricesForCar.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour4)?.Price ?? 0m;
                    var p8 = pricesForCar.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour8)?.Price ?? 0m;
                    var p12 = pricesForCar.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour12)?.Price ?? 0m;
                    var p24 = pricesForCar.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour24)?.Price ?? 0m;

                    var total = RentalFeeCalculator.CalcTotalPrice(o.PickupDateTime, o.ReturnDateTime, p4, p8, p12, p24);
                    if (total > 0m)
                    {
                        computedTotalByOrderId[o.Id] = total;
                    }
                }
            }

            var payments = await _unitOfWork.Payments.QueryableAsync()
                .AsNoTracking()
                .Where(p => orderIds.Contains(p.RentalOrderId)
                            && p.Provider == EnumPaymentProvider.Vnpay)
                .Select(p => new
                {
                    p.RentalOrderId,
                    p.Status,
                    p.Amount,
                    p.Purpose
                })
                .ToListAsync(cancellationToken);

            var paidDepositByOrder = payments
                .Where(p => p.Status == EnumPaymentStatus.Paid
                            && p.Purpose == EnumPaymentPurpose.Deposit)
                .GroupBy(p => p.RentalOrderId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var paidRentalFeeByOrder = payments
                .Where(p => p.Status == EnumPaymentStatus.Paid
                            && p.Purpose == EnumPaymentPurpose.RentalFee)
                .GroupBy(p => p.RentalOrderId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var hasUnfinishedRentalFeeByOrder = payments
                .Where(p => (p.Status == EnumPaymentStatus.Created
                             || p.Status == EnumPaymentStatus.Pending)
                            && p.Purpose == EnumPaymentPurpose.RentalFee)
                .GroupBy(p => p.RentalOrderId)
                .ToDictionary(g => g.Key, g => true);

            methodResult.Result = new MyActiveRentalsResult
            {
                Items = orders.Select(o =>
                {
                    var carName = GetCarName(o.CarId);
                    var primaryImageUrl = GetPrimaryImage(o.CarId);

                    var totalRentalFee = o.TotalRentalFee;
                    if (totalRentalFee <= 0m && computedTotalByOrderId.TryGetValue(o.Id, out var computedTotal))
                    {
                        totalRentalFee = computedTotal;
                    }

                    var paidRentalFee = paidRentalFeeByOrder
                        .TryGetValue(o.Id, out var pr) ? pr : 0m;

                    var requiredRentalFee = totalRentalFee - o.DepositAmount;
                    if (requiredRentalFee < 0) requiredRentalFee = 0m;

                    var remaining = requiredRentalFee - paidRentalFee;
                    if (remaining < 0) remaining = 0m;

                    var hasUnfinished = hasUnfinishedRentalFeeByOrder
                        .TryGetValue(o.Id, out var hf) && hf;

                    return new MyActiveRentalsItem
                    {
                        RentalOrderId = o.Id,
                        CarId = o.CarId,
                        CarName = carName,
                        PrimaryImageUrl = primaryImageUrl,
                        PickupDateTime = o.PickupDateTime,
                        ReturnDateTime = o.ReturnDateTime,
                        DepositAmount = o.DepositAmount,
                        TotalRentalFee = totalRentalFee,
                        PaidRentalFeeAmount = paidRentalFee,
                        RemainingAmount = remaining,
                        HasUnfinishedRentalFeePayment = hasUnfinished,
                        Status = o.Status.ToString(),
                        CreatedAt = o.CreatedAt
                    };
                }).ToList()
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}