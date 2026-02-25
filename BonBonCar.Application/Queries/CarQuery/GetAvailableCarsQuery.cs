using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQuery
{
    public class GetAvailableCarsQuery : IRequest<MethodResult<IList<CarModel>>>
    {
        public int Take { get; set; } = 12;
    }

    public class GetAvailableCarsQueryHandler : IRequestHandler<GetAvailableCarsQuery, MethodResult<IList<CarModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<CarModel>>> Handle(GetAvailableCarsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<CarModel>>();
            var take = request.Take <= 0 ? 12 : Math.Min(request.Take, 50);
            var cars = _unitOfWork.Cars.QueryableAsync();
            var brands = _unitOfWork.Brands.QueryableAsync();
            var models = _unitOfWork.Models.QueryableAsync();

            var query =
                from c in cars
                where c.Status == EnumCarStatus.Available
                join m in models on c.ModelId equals m.Id
                join b in brands on m.BrandId equals b.Id
                orderby c.CreatedAt descending
                select new CarModel
                {
                    CarId = c.Id,
                    CarName = b.Name + " " + m.Name + " " + c.Year,
                    PickupAddress = c.PickupAddress,
                    PrimaryImageUrl = _unitOfWork.CarImages.QueryableAsync()
                        .AsNoTracking()
                        .Where(i => i.CarId == c.Id)
                        .OrderByDescending(i => i.IsPrimary)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    Price4h = _unitOfWork.CarPrices.QueryableAsync()
                        .AsNoTracking()
                        .Where(p => p.CarId == c.Id && p.RentalDuration == EnumRentalDuration.hour4)
                        .Select(p => (decimal?)p.Price)
                        .FirstOrDefault() ?? 0m,
                    Price24h = _unitOfWork.CarPrices.QueryableAsync()
                        .AsNoTracking()
                        .Where(p => p.CarId == c.Id && p.RentalDuration == EnumRentalDuration.hour24)
                        .Select(p => (decimal?)p.Price)
                        .FirstOrDefault() ?? 0m,
                };

            var items = await query.Take(take).ToListAsync(cancellationToken);
            methodResult.Result = items;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
