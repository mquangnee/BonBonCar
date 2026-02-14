using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.CmdModels.CarCmdModels;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BonBonCar.Application.Commands.CarCmd
{
    public class CreateCarCmd : CreateCarCmdModel, IRequest<MethodResult<bool>>
    {
    }

    public class CreateCarCmdHandler : IRequestHandler<CreateCarCmd, MethodResult<bool>>
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateCarCmdHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<bool>> Handle(CreateCarCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }

            var model = await _unitOfWork.Models.GetByIdAsync(request.ModelId);
            if (model == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.ModelId));
                return methodResult;
            }

            var carExist = await _unitOfWork.Cars.GetByLicensePlate(request.LicensePlate ?? string.Empty);
            if (carExist != null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.LicensePlate));
                return methodResult;
            }

            var prices = string.IsNullOrWhiteSpace(request.Prices) ? new List<CarPriceModel>() : System.Text.Json.JsonSerializer.Deserialize<List<CarPriceModel>>(request.Prices);

            var car = new Car
            {
                UserId = userId,
                ModelId = request.ModelId,
                Location = request.Location?.Trim().ToUpperInvariant(),
                Year = request.Year,
                LicensePlate =request.LicensePlate?.Trim(),
                PickupAddress = request.PickupAddress?.Trim(),
                Features = request.Features
            };
            await _unitOfWork.Cars.AddAsync(car);

            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars", car.Id.ToString());

            if (!Directory.Exists(uploadRoot))
                Directory.CreateDirectory(uploadRoot);

            bool isFirstImage = true;

            if (request.Images != null && request.Images.Count > 0)
            {
                foreach (var file in request.Images)
                {
                    if (file.Length == 0) continue;
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var fullPath = Path.Combine(uploadRoot, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream, cancellationToken);

                    var imageUrl = $"/images/cars/{car.Id}/{fileName}";

                    var carImage = new CarImage
                    {
                        CarId = car.Id,
                        ImageUrl = imageUrl,
                        IsPrimary = isFirstImage
                    };
                    await _unitOfWork.CarImages.AddAsync(carImage);

                    isFirstImage = false;
                }
            }
            
            foreach (var price in prices)
            {
                var carPrice = new CarPrice
                {
                    CarId = car.Id,
                    RentalDuration = price.RentalDuration,
                    Price = price.Price
                };
                await _unitOfWork.CarPrices.AddAsync(carPrice);
            }

            var result = _unitOfWork.SaveChanges();
            if (result == 0)
            {
                methodResult.AddErrorBadRequest("Đăng tải xe cho thuê thất bại!");
                return methodResult;
            }
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status201Created;
            return methodResult;
        }
    }
}
