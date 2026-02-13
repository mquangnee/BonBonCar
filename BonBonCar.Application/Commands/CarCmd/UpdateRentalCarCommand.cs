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

            var old = await _unitOfWork.CarImages.QueryableAsync().Where(x => x.CarId == request.CarId).ToListAsync(cancellationToken);
            var keep = (request.KeepImages ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            var newCount = request.Images?.Count ?? 0;
            if (keep.Count + newCount == 0)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.InValidFormat), "Phải có ít nhất 1 ảnh");
                return methodResult;
            }

            var toDelete = old.Where(x => !keep.Contains(x.ImageUrl)).ToList();
            var carFolder = Path.Combine(_env.WebRootPath, "images", "cars", request.CarId.ToString());

            foreach (var img in toDelete)
            {
                var fileName = Path.GetFileName(img.ImageUrl);
                var physicalPath = Path.Combine(carFolder, fileName);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
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

            var thumbnailUrl = keep.FirstOrDefault() ?? addedUrls.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                var remain = await _unitOfWork.CarImages.QueryableAsync().Where(x => x.CarId == request.CarId).ToListAsync(cancellationToken);

                foreach (var img in remain)
                {
                    img.IsPrimary = (img.ImageUrl == thumbnailUrl);
                }            
            }

            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
