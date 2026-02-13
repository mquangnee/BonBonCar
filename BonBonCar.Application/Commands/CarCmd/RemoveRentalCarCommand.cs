using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Commands.CarCmd
{
    public class RemoveRentalCarCommand : IRequest<MethodResult<bool>>
    {
        public Guid CarId;
    }

    public class RemoveRentalCarCommandHandler : IRequestHandler<RemoveRentalCarCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRentalCarCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(RemoveRentalCarCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if (car == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId));
                return methodResult;
            }

            if (car.Status == EnumCarStatus.Rented)
            {
                methodResult.AddErrorBadRequest("Xe đang được cho thuê", nameof(car.Status));
                return methodResult;
            }
            _unitOfWork.Cars.DeleteAsync(car);

            var carImages = await _unitOfWork.CarImages.QueryableAsync().Where(i => i.CarId == request.CarId).ToListAsync();
            if (carImages == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            foreach (var image in carImages)
            {
                _unitOfWork.CarImages.DeleteAsync(image);
            }

            var carPrices = await _unitOfWork.CarPrices.QueryableAsync().Where(i => i.CarId == request.CarId).ToListAsync();
            if (carPrices == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            foreach (var price in carPrices)
            {
                _unitOfWork.CarPrices.DeleteAsync(price);
            }

            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        } 
    }
}
