using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Commands.AdminCmd
{
    public class SetCarStatusCommand : IRequest<MethodResult<bool>>
    {
        public Guid CarId { get; set; }
        public EnumCarStatus Status { get; set; }
    }

    public class SetCarStatusCommandHandler : IRequestHandler<SetCarStatusCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetCarStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(SetCarStatusCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
            if (car == null)
            {
                methodResult.AddErrorBadRequest("CarNotFound");
                return methodResult;
            }

            if (request.Status == EnumCarStatus.Disabled && car.Status == EnumCarStatus.Rented)
            {
                methodResult.AddErrorBadRequest("CarIsRented");
                return methodResult;
            }

            car.Status = request.Status;
            _unitOfWork.Cars.Update(car);
            _unitOfWork.SaveChanges();

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

