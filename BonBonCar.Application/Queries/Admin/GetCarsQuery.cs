using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.QueryModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.Admin
{
    public class GetCarsQuery : IRequest<MethodResult<ListCarsModel>>
    {
        public EnumCarStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetCarsQueryModel : IRequestHandler<GetCarsQuery, MethodResult<ListCarsModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarsQueryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<ListCarsModel>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ListCarsModel>();

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

            var carsQuery = _unitOfWork.Cars.QueryableAsync().AsNoTracking();

            if (request.Status.HasValue)
            {
                var status = request.Status.Value;
                carsQuery = carsQuery.Where(c => c.Status == status);
            }

            var totalCount = await carsQuery.CountAsync(cancellationToken);

            var cars = await carsQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (cars.Count == 0)
            {
                methodResult.Result = new ListCarsModel
                {
                    Items = new List<CarItemModel>(),
                    TotalCount = 0,
                    Page = page,
                    PageSize = pageSize
                };
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            var carIds = cars.Select(c => c.Id).ToList();
            var modelIds = cars.Select(c => c.ModelId).Distinct().ToList();

            var models = await _unitOfWork.Models.QueryableAsync().AsNoTracking()
                .Where(m => modelIds.Contains(m.Id))
                .ToListAsync(cancellationToken);

            var brandIds = models.Select(m => m.BrandId).Distinct().ToList();
            var brands = await _unitOfWork.Brands.QueryableAsync().AsNoTracking()
                .Where(b => brandIds.Contains(b.Id))
                .ToListAsync(cancellationToken);

            var images = await _unitOfWork.CarImages.QueryableAsync().AsNoTracking()
                .Where(i => carIds.Contains(i.CarId))
                .ToListAsync(cancellationToken);

            var modelById = models.ToDictionary(x => x.Id, x => x);
            var brandById = brands.ToDictionary(x => x.Id, x => x);

            var imagesByCar = images
                .Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl))
                .GroupBy(i => i.CarId)
                .ToDictionary(g => g.Key, g => g.ToList());

            string? GetPrimaryImage(Guid carId)
            {
                if (!imagesByCar.TryGetValue(carId, out var list) || list == null || list.Count == 0)
                    return null;

                return list
                    .OrderByDescending(x => x.IsPrimary)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault();
            }

            string GetCarName(Guid carId, Guid modelId, int year)
            {
                modelById.TryGetValue(modelId, out var model);
                var brand = model != null && brandById.TryGetValue(model.BrandId, out var b) ? b : null;

                var brandName = brand?.Name?.Trim();
                var modelName = model?.Name?.Trim();

                var name = string.Join(" ",
                    new[] { brandName, modelName, year.ToString() }
                        .Where(x => !string.IsNullOrWhiteSpace(x)));

                return string.IsNullOrWhiteSpace(name) ? $"Xe {year}" : name;
            }

            var items = cars.Select(car => new CarItemModel
            {
                CarId = car.Id,
                CarName = GetCarName(car.Id, car.ModelId, car.Year),
                Status = car.Status,
                PickupAddress = car.PickupAddress,
                PrimaryImageUrl = GetPrimaryImage(car.Id),
                CreatedAt = car.CreatedAt
            }).ToList();

            methodResult.Result = new ListCarsModel
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

