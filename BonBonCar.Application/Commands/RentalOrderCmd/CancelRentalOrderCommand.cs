using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.Enums.RentalOrder;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Commands.Rentals
{
    public class CancelRentalOrderCommand : IRequest<MethodResult<CancelRentalResponse>>
    {
        public Guid CustomerId { get; set; }
        public Guid RentalOrderId { get; set; }
        public DateTime TimeNow { get; set; } = DateTime.Now;
    }

    public class CancelRentalOrderCommandHandler : IRequestHandler<CancelRentalOrderCommand, MethodResult<CancelRentalResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelRentalOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<CancelRentalResponse>> Handle(CancelRentalOrderCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CancelRentalResponse>();
            if (request.CustomerId == Guid.Empty || request.RentalOrderId == Guid.Empty)
            {
                var propertyName = request.CustomerId == Guid.Empty ? request.CustomerId : request.RentalOrderId;
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Required), nameof(propertyName), propertyName);
                return methodResult;
            }
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId);
            if (order is null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.RentalOrderId), request.RentalOrderId);
                return methodResult;
            }
            if (order.CustomerId != request.CustomerId)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized), nameof(request.CustomerId), request.CustomerId);
                return methodResult;
            }
            if (order.Status == EnumRentalOrderStatus.HoldFailed || order.Status == EnumRentalOrderStatus.HoldExpired || order.Status == EnumRentalOrderStatus.Paid)
            {
                methodResult.AddErrorBadRequest(nameof(EnumRentalOrderErrorCode.InvalidRentalOrderStatus), nameof(order.Status), order.Status);
                return methodResult;
            }
            if (request.TimeNow >= order.PickupDateTime)
            {
                methodResult.AddErrorBadRequest(nameof(EnumRentalOrderErrorCode.PickupTimeReached), nameof(request.TimeNow), request.TimeNow);
                return methodResult;
            }
            var car = await _unitOfWork.Cars.GetByIdAsync(order.CarId);
            if (car == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(order.CarId), order.CarId);
                return methodResult;
            }
            car.Status = EnumCarStatus.Available;
            order.MarkCanceled();
            _unitOfWork.Cars.Update(car);
            _unitOfWork.RentalOrders.Update(order);
            var pendingPayments = await _unitOfWork.Payments.QueryableAsync()
                .Where(p => p.RentalOrderId == order.Id && (p.Status == EnumPaymentStatus.Created || p.Status == EnumPaymentStatus.Pending))
                .ToListAsync(cancellationToken);
            foreach (var p in pendingPayments)
            {
                p.MarkExpired();
                _unitOfWork.Payments.Update(p);
            }
            _unitOfWork.SaveChanges();
            methodResult.Result = new CancelRentalResponse
            {
                RentalOrderId = order.Id,
                Status = order.Status.ToString(),
                UpdatedAt = DateTime.Now
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}