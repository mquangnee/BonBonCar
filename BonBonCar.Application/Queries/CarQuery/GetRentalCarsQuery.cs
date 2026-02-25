using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BonBonCar.Application.Queries.CarQuery
{
    public class GetRentalCarsQuery : IRequest<MethodResult<IList<RentalCarModel>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public EnumCarStatus? Status { get; set; }
        public string? Keyword { get; set; }
    }

    public class GetRentalCarsQueryHandler : IRequestHandler<GetRentalCarsQuery, MethodResult<IList<RentalCarModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetRentalCarsQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<IList<RentalCarModel>>> Handle(GetRentalCarsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<RentalCarModel>>();
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }

            var rentalCars = await _unitOfWork.Cars.QueryableAsync().Where(c => c.UserId == userId).OrderByDescending(c => c.CreatedAt).ToListAsync();
            if (rentalCars == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var rentalCarsList = new List<RentalCarModel>();
            foreach (var car in rentalCars)
            {
                var model = await _unitOfWork.Models.GetByIdAsync(car.ModelId);
                if (model == null)
                {
                    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(car.ModelId));
                    return methodResult;
                }
                var brand = await _unitOfWork.Brands.GetByIdAsync(model.BrandId);
                if (brand == null)
                {
                    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(model.BrandId));
                    return methodResult;
                }
                var thumbnailImage = await _unitOfWork.CarImages.QueryableAsync().FirstOrDefaultAsync(i => i.CarId == car.Id && i.IsPrimary == true);
                if (thumbnailImage == null)
                {
                    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                    return methodResult;
                }
                var rentalCar = new RentalCarModel
                {
                    CarId = car.Id,
                    BrandName = brand.Name,
                    ModelName = model.Name,
                    Year = car.Year,
                    Location = car.Location,
                    LicensePlate = car.LicensePlate,
                    ThumbnailUrl = thumbnailImage.ImageUrl,
                    Status = car.Status
                };
                rentalCarsList.Add(rentalCar);
            }

            if (request.Status.HasValue)
            {
                rentalCarsList = rentalCarsList.Where(c => c.Status == request.Status).ToList();
            }
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                rentalCarsList = rentalCarsList.Where(c => c.BrandName.ToLower().Contains(keyword) || c.ModelName.ToLower().Contains(keyword) || c.LicensePlate.ToLower().Contains(keyword)).ToList();
            }
            rentalCarsList = rentalCarsList
                                .Skip((request.Page - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToList();
            methodResult.Result = rentalCarsList;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
