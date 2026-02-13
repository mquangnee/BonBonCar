using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Commands.CarCmd
{
    public class ChangeCarStatusCommand : IRequest<MethodResult<bool>>
    {
        public Guid CarId { get; set; }
    }

    public class ChangeCarStatusCommandHandler : IRequestHandler<ChangeCarStatusCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeCarStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(ChangeCarStatusCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if (car == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarId));
                return methodResult;
            }
            car.Status = car.Status == EnumCarStatus.Inactive ? EnumCarStatus.Available : EnumCarStatus.Inactive;
            _unitOfWork.Cars.Update(car);
            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
