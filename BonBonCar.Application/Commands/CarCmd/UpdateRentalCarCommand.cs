using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.CmdModels.CarCmdModels;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Commands.CarCmd
{
    public class UpdateRentalCarCommand : UpdateRentalCarCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class UpdateRentalCarCommandHandler : IRequestHandler<UpdateRentalCarCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public UpdateRentalCarCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<MethodResult<bool>> Handle(UpdateRentalCarCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            
            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if (car == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId));
                return methodResult;
            }
            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars", car.Id.ToString());
            car.Features = request.Features;
            _unitOfWork.Cars.Update(car);

            var prices = string.IsNullOrWhiteSpace(request.Prices) ? new List<CarPriceModel>() : System.Text.Json.JsonSerializer.Deserialize<List<CarPriceModel>>(request.Prices);
            var carPrices = await _unitOfWork.CarPrices.QueryableAsync().Where(p => p.CarId == request.CarId).ToListAsync();
            if (carPrices == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            foreach (var price in carPrices)
            {
                foreach (var item in prices)
                {
                    if (price.RentalDuration == item.RentalDuration)
                    {
                        price.Price = item.Price;
                    }
                }
                _unitOfWork.CarPrices.Update(price);
            }

            if (request.Images != null && request.Images.Count > 0)
            {
                var carFolderPhysical = Path.Combine(_env.WebRootPath, "images", "cars", request.CarId.ToString());
                Directory.CreateDirectory(carFolderPhysical);

                foreach (var file in Directory.GetFiles(carFolderPhysical))
                {
                    System.IO.File.Delete(file);
                }

                var oldImages = await _unitOfWork.CarImages.QueryableAsync().Where(x => x.CarId == request.CarId).ToListAsync(cancellationToken);

                foreach (var img in oldImages)
                {
                    _unitOfWork.CarImages.DeleteAsync(img);
                }

            }

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
            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
