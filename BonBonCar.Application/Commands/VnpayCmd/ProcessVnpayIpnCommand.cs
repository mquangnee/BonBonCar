using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.Enums.RentalOrder;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Commands.VnpayCmd
{
    public class ProcessVnpayIpnCommand : IRequest<MethodResult<VnpayIpnResponse>>
    {
        public IQueryCollection Query { get; }
        public ProcessVnpayIpnCommand(IQueryCollection query)
        {
            Query = query;
        }
    }
    public class ProcessVnpayIpnCommandHandler : IRequestHandler<ProcessVnpayIpnCommand, MethodResult<VnpayIpnResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnpayGateway _vpnGateway;

        public ProcessVnpayIpnCommandHandler(IUnitOfWork unitOfWork, IVnpayGateway vnpayGateway)
        {
            _unitOfWork = unitOfWork;
            _vpnGateway = vnpayGateway;
        }

        public async Task<MethodResult<VnpayIpnResponse>> Handle(ProcessVnpayIpnCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<VnpayIpnResponse>();

            VnpayIpnResponse Resp(string code, string msg) => new() { RspCode = code, Message = msg };
            var vnpayResposne = _vpnGateway.ValidateCallback(request.Query);
            if (!vnpayResposne.IsValidSignature || string.IsNullOrWhiteSpace(vnpayResposne.TxnRef))
            {
                methodResult.Result = Resp("97", "Invalid signature");
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            var payment = await _unitOfWork.Payments.QueryableAsync()
                .Include(p => p.RentalOrder)
                .FirstOrDefaultAsync(p => p.TxnRef == vnpayResposne.TxnRef, cancellationToken);
            if (payment is null)
            {
                methodResult.Result = Resp("01", "Order not found");
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            if (payment.Status == EnumPaymentStatus.Paid)
            {
                if (payment.RentalOrder != null && payment.RentalOrder.Status != EnumRentalOrderStatus.Held)
                {
                    payment.RentalOrder.MarkHeld();
                    _unitOfWork.SaveChanges();
                }

                methodResult.Result = Resp("00", "Confirm Success");
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            if (DateTime.Now > payment.ExpiresAt)
            {
                payment.MarkExpired();
                payment.RentalOrder?.MarkHoldExpired();
                _unitOfWork.SaveChanges();
                methodResult.Result = Resp("00", "Confirm Success");
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }
            var responseCode = vnpayResposne.ResponseCode ?? "99";
            var transNo = vnpayResposne.TransactionNo ?? "";
            var raw = vnpayResposne.RawQueryString;

            if (responseCode == "00")
            {
                payment.MarkPaid(transNo, responseCode, raw);

                if (payment.RentalOrder != null)
                {
                    if (payment.RentalOrder.Status == EnumRentalOrderStatus.HoldPending || payment.RentalOrder.Status == EnumRentalOrderStatus.Created)
                    {
                        payment.RentalOrder.MarkHeld();
                    }
                }
                _unitOfWork.SaveChanges();
                methodResult.Result = Resp("00", "Confirm Success");
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }

            payment.MarkFailed(responseCode, raw);
            payment.RentalOrder?.MarkHoldFailed();
            _unitOfWork.SaveChanges();

            methodResult.Result = Resp("00", "Confirm Success");
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
