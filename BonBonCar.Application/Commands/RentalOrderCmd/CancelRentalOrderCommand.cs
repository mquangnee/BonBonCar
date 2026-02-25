using BonBonCar.Application.Common;
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
        public DateTime NowLocal { get; set; } = DateTime.Now;
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
                methodResult.AddErrorBadRequest("Dữ liệu không hợp lệ!");
                return methodResult;
            }
            var order = await _unitOfWork.RentalOrders.GetByIdAsync(request.RentalOrderId);
            if (order is null)
            {
                methodResult.AddErrorBadRequest("Không tìm thấy đơn thuê!");
                return methodResult;
            }
            if (order.CustomerId != request.CustomerId)
            {
                methodResult.AddErrorBadRequest("Bạn không có quyền hủy đơn này!");
                return methodResult;
            }
            if (order.Status == EnumRentalOrderStatus.HoldFailed || order.Status == EnumRentalOrderStatus.HoldExpired)
            {
                methodResult.AddErrorBadRequest("Đơn thuê không thể hủy ở trạng thái hiện tại!");
                return methodResult;
            }

            var hasPaidRentalFee = await _unitOfWork.Payments.QueryableAsync().AsNoTracking().AnyAsync(
                p => p.RentalOrderId == order.Id
                && p.Provider == EnumPaymentProvider.Vnpay
                && p.Purpose == EnumPaymentPurpose.RentalFee
                && p.Status == EnumPaymentStatus.Paid, cancellationToken);
            if (hasPaidRentalFee)
            {
                methodResult.AddErrorBadRequest("Đơn đã thanh toán tiền thuê, không thể hủy!");
                return methodResult;
            }
            if (request.NowLocal >= order.PickupDateTime)
            {
                methodResult.AddErrorBadRequest("Đã đến giờ nhận xe, không thể hủy!");
                return methodResult;
            }
            order.MarkHoldExpired();
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