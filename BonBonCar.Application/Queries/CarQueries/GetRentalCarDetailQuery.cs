using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQueries
{
    public class GetRentalCarDetailQuery : IRequest<MethodResult<RentalCarModel>>
    {
        public Guid CarId { get; set; }
    }

    public class GetRentalCarDetailQueryHandler : IRequestHandler<GetRentalCarDetailQuery, MethodResult<RentalCarModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRentalCarDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<RentalCarModel>> Handle(GetRentalCarDetailQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RentalCarModel>();
            
            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if (car == null) 
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId));
                return methodResult;
            }

            var carImages = await _unitOfWork.CarImages.QueryableAsync().Where(i => i.CarId == request.CarId).ToListAsync();
            var carPrices = await _unitOfWork.CarPrices.QueryableAsync().Where(p => p.CarId == request.CarId).ToListAsync();
            var model = await _unitOfWork.Models.GetByIdAsync(car.ModelId);
            if (carImages == null || carPrices == null || model == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var brand = await _unitOfWork.Brands.GetByIdAsync(model.BrandId);
            if (brand == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var thumbnailImage = carImages.FirstOrDefault(i => i.IsPrimary == true);
            if (thumbnailImage == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var basePrices = await _unitOfWork.BasePrices.QueryableAsync().Where(p => p.CarType == model.VehicleType).ToListAsync();

            var listFeature = new List<string>();
            var listPrice = new List<CarPriceModel>();
            var listImage = new List<string>();
            var listBasePrices = new List<BasePriceModel>();

            foreach (var basePrice in basePrices)
            {
                var basePriceModel = new BasePriceModel
                {
                    RentalDuration = basePrice.RentalDuration,
                    BasePrice = basePrice.BasePrice
                };
                listBasePrices.Add(basePriceModel);
            }
            foreach (var feature in car.Features)
            {
                listFeature.Add(feature);
            }
            foreach (var price in carPrices)
            {
                var carPriceModel = new CarPriceModel
                {
                    RentalDuration = price.RentalDuration,
                    Price = price.Price
                };
                listPrice.Add(carPriceModel);
            }
            foreach (var image in carImages)
            {
                listImage.Add(image.ImageUrl);
            }

            var rentalCar = new RentalCarModel
            {
                CarId = request.CarId,
                BrandName = brand.Name,
                ModelName = model.Name,
                Year = car.Year,
                Location = car.Location,
                PickupAddress = car.PickupAddress,
                LicensePlate = car.LicensePlate,
                ThumbnailUrl = thumbnailImage.ImageUrl,
                Features = listFeature,
                BasePrices = listBasePrices,
                Prices = listPrice,
                Images = listImage,
                Status = car.Status
            };
            methodResult.Result = rentalCar;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
