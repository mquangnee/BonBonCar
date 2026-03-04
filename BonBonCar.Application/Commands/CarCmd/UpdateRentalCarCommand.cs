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
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId), request.CarId);
                return methodResult;
            }
            car.Features = request.Features;
            car.Location = request.Location;
            car.PickupAddress = request.PickupAddress;
            _unitOfWork.Cars.Update(car);

            var prices = string.IsNullOrWhiteSpace(request.Prices) ? new List<CarPriceModel>() : System.Text.Json.JsonSerializer.Deserialize<List<CarPriceModel>>(request.Prices);
            if (prices == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Prices), request.Prices);
                return methodResult;
            }
            var carPrices = await _unitOfWork.CarPrices.QueryableAsync().Where(p => p.CarId == request.CarId).ToListAsync();
            if (carPrices == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId), request.CarId);
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

            var oldImages = await _unitOfWork.CarImages.QueryableAsync().Where(x => x.CarId == request.CarId).ToListAsync(cancellationToken);
            var keepImages = (request.KeepImages ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            if (keepImages.Count == 0 && request.Images?.Count == 0)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Required), nameof(request.KeepImages), request.KeepImages);
                return methodResult;
            }
            var toDelete = oldImages.Where(x => !keepImages.Contains(x.ImageUrl)).ToList();
            var carFolder = Path.Combine(_env.WebRootPath, "images", "cars", request.CarId.ToString());
            foreach (var img in toDelete)
            {
                var fileName = Path.GetFileName(img.ImageUrl);
                if (fileName == null)
                {
                    continue;
                }
                var physicalPath = Path.Combine(carFolder, fileName);
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
                _unitOfWork.CarImages.DeleteAsync(img);
            }
            var addedUrls = new List<string>();
            if (request.Images != null && request.Images.Count > 0)
            {
                Directory.CreateDirectory(carFolder);

                foreach (var file in request.Images)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var savePath = Path.Combine(carFolder, fileName);

                    using var stream = new FileStream(savePath, FileMode.Create);
                    await file.CopyToAsync(stream, cancellationToken);

                    var url = $"/images/cars/{request.CarId}/{fileName}";
                    addedUrls.Add(url);

                    await _unitOfWork.CarImages.AddAsync(new CarImage
                    {
                        CarId = request.CarId,
                        ImageUrl = url,
                        IsPrimary = false,
                    });
                }
            }
            
            var thumbnailUrl = keepImages.FirstOrDefault() ?? addedUrls.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                var currentPrimary = await _unitOfWork.CarImages.QueryableAsync().FirstOrDefaultAsync(i => i.CarId == request.CarId && i.IsPrimary);
                if (currentPrimary != null && currentPrimary.ImageUrl != thumbnailUrl)
                {
                    currentPrimary.IsPrimary = false;
                }
                var thumbnailImage = await _unitOfWork.CarImages.QueryableAsync().FirstOrDefaultAsync(i => i.CarId == request.CarId && i.ImageUrl == thumbnailUrl);
                if (thumbnailImage != null)
                {
                    thumbnailImage.IsPrimary = true;
                }     
            }

            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
