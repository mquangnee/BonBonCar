using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.Enums.RentalOrder;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using BonBonCar.Infrastructure.Services.GoogleDocumentAI;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BonBonCar.Application.Commands.Rentals
{
    public class CreateRentalFeePaymentCommand : IRequest<MethodResult<CreateRentalFeePaymentResponse>>
    {
        public Guid CustomerId { get; set; }
        public string ClientIp { get; set; } = "127.0.0.1";
        public Guid RentalOrderId { get; set; }
        public string VnpayReturnUrl { get; set; } = string.Empty;
        public CreateRentalFeePaymentRequest Request { get; set; } = new();
    }

    public class CreateRentalFeePaymentCommandHandler : IRequestHandler<CreateRentalFeePaymentCommand, MethodResult<CreateRentalFeePaymentResponse>>
    {
        private readonly IVnpayGateway _vnpayGateway;
        private readonly VnpayOptions _vnpayOptions;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRentalFeePaymentCommandHandler(IVnpayGateway vnpayGateway, IOptions<VnpayOptions> vnpayOptions, IUnitOfWork unitOfWork)
        {
            _vnpayGateway = vnpayGateway;
            _vnpayOptions = vnpayOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<CreateRentalFeePaymentResponse>> Handle(CreateRentalFeePaymentCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CreateRentalFeePaymentResponse>();

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
                methodResult.AddErrorBadRequest("Bạn không có quyền thanh toán đơn này!");
                return methodResult;
            }

            if (order.Status != EnumRentalOrderStatus.Held)
            {
                methodResult.AddErrorBadRequest("Chỉ được thanh toán tiền thuê khi đơn đã giữ chỗ thành công (Held).");
                return methodResult;
            }

            var now = DateTime.Now;
            if (now < order.PickupDateTime)
            {
                methodResult.AddErrorBadRequest("Chỉ được thanh toán tiền thuê khi đã đến thời gian bắt đầu thuê xe.");
                return methodResult;
            }

            var requiredRentalFee = order.TotalRentalFee - order.DepositAmount;
            if (requiredRentalFee < 0) requiredRentalFee = 0m;

            var hasUnfinished = await _unitOfWork.Payments.QueryableAsync()
                .AsNoTracking()
                .AnyAsync(p => p.RentalOrderId == order.Id
                               && p.Provider == EnumPaymentProvider.Vnpay
                               && p.Purpose == EnumPaymentPurpose.RentalFee
                               && (p.Status == EnumPaymentStatus.Created || p.Status == EnumPaymentStatus.Pending), ct);

            if (hasUnfinished)
            {
                methodResult.AddErrorBadRequest("Đang có giao dịch thanh toán tiền thuê chưa hoàn tất!");
                return methodResult;
            }

            var paidRentalFee = await _unitOfWork.Payments.QueryableAsync()
                .AsNoTracking()
                .Where(p => p.RentalOrderId == order.Id
                            && p.Provider == EnumPaymentProvider.Vnpay
                            && p.Purpose == EnumPaymentPurpose.RentalFee
                            && p.Status == EnumPaymentStatus.Paid)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

            var remaining = requiredRentalFee - paidRentalFee;
            if (remaining <= 0)
            {
                methodResult.AddErrorBadRequest("Đơn đã thanh toán đủ tiền thuê!");
                return methodResult;
            }

            var amountRounded = decimal.Round(remaining, 0, MidpointRounding.AwayFromZero);
            if (amountRounded <= 0)
            {
                methodResult.AddErrorBadRequest("Số tiền còn lại không hợp lệ!");
                return methodResult;
            }

            var expire = now.AddMinutes(15);

            var payment = new Payment
            {
                RentalOrderId = order.Id,
                Amount = amountRounded,
                TxnRef = TxnRefGenerator.NewTxnRef(order.Id),
                Provider = EnumPaymentProvider.Vnpay,
                Status = EnumPaymentStatus.Created,
                CreatedAt = now,
                ExpiresAt = expire,
                Purpose = EnumPaymentPurpose.RentalFee
            };
            payment.MarkPending();

            await _unitOfWork.Payments.AddAsync(payment);
            _unitOfWork.SaveChanges();

            var orderInfo = $"Thanh toán chi phí thuê xe - ID: {order.Id}";

            var paymentUrl = _vnpayGateway.BuildPaymentUrl(new VnpayBuildUrlRequest(
                txnRef: payment.TxnRef,
                amountVnd: payment.Amount,
                orderInfo: orderInfo,
                ipAddress: request.ClientIp,
                createDateLocal: now,
                expireDateLocal: expire,
                returnUrl: string.IsNullOrWhiteSpace(request.VnpayReturnUrl) ? _vnpayOptions.ReturnUrl : request.VnpayReturnUrl,
                ipnUrl: _vnpayOptions.IpnUrl ?? string.Empty
            ));

            methodResult.Result = new CreateRentalFeePaymentResponse
            {
                RentalOrderId = order.Id,
                PaymentId = payment.Id,
                TxnRef = payment.TxnRef,
                Amount = payment.Amount,
                ExpiresAt = payment.ExpiresAt,
                PaymentUrl = paymentUrl
            };

            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}