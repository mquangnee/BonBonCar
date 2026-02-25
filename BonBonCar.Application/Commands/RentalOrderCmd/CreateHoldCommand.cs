using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using BonBonCar.Infrastructure.Services.GoogleDocumentAI;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BonBonCar.Application.Commands.RentalOrderCmd
{
    public class CreateHoldCommand : IRequest<MethodResult<CreateHoldResponse>>
    {
        public Guid CustomerId { get; set; }
        public string ClientIp { get; set; } = "127.0.0.1";
        public string VnpayReturnUrl { get; set; } = string.Empty;
        public CreateHoldRequest Request { get; set; } = new();
    }

    public class CreateHoldCommandHandler : IRequestHandler<CreateHoldCommand, MethodResult<CreateHoldResponse>>
    {
        private readonly IVnpayGateway _vnpayGateway;
        private readonly VnpayOptions _vnpayOptions;
        private readonly IUnitOfWork _unitOfWork;

        const decimal DEPOSIT = 500_000m;

        public CreateHoldCommandHandler(IVnpayGateway vnpayGateway, IOptions<VnpayOptions> vnpayOptions, IUnitOfWork unitOfWork)
        {
            _vnpayGateway = vnpayGateway;
            _vnpayOptions = vnpayOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<CreateHoldResponse>> Handle(CreateHoldCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CreateHoldResponse>();

            var pickupTime = request.Request.PickupAt;
            var returnTime = request.Request.ReturnAt;

            if (pickupTime >= returnTime)
                throw new ArgumentException("pickupAt must be earlier than returnAt.");
            var now = DateTime.Now;
            var expire = now.AddMinutes(15);

            var carPrices = await _unitOfWork.CarPrices.QueryableAsync()
                .AsNoTracking()
                .Where(p => p.CarId == request.Request.CarId)
                .ToListAsync(ct);

            if (!carPrices.Any())
            {
                methodResult.AddErrorBadRequest("Xe chưa có cấu hình giá thuê!");
                return methodResult;
            }

            var p4 = carPrices.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour4)?.Price ?? 0m;
            var p8 = carPrices.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour8)?.Price ?? 0m;
            var p12 = carPrices.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour12)?.Price ?? 0m;
            var p24 = carPrices.FirstOrDefault(x => x.RentalDuration == EnumRentalDuration.hour24)?.Price ?? 0m;

            var totalPrice = RentalFeeCalculator.CalcTotalPrice(pickupTime, returnTime, p4, p8, p12, p24);
            if (totalPrice <= 0)
            {
                methodResult.AddErrorBadRequest("Không thể tính được tổng tiền thuê!");
                return methodResult;
            }

            var order = new RentalOrder
            {
                CarId = request.Request.CarId,
                CustomerId = request.CustomerId,
                PickupDateTime = pickupTime,
                ReturnDateTime = returnTime,
                DepositAmount = DEPOSIT,
                TotalRentalFee = totalPrice,
                CreatedAt = now
            };
            order.MarkHoldPending();

            var payment = new Payment
            {
                RentalOrderId = order.Id,
                Amount = DEPOSIT,
                TxnRef = TxnRefGenerator.NewTxnRef(order.Id),
                Provider = EnumPaymentProvider.Vnpay,
                Status = EnumPaymentStatus.Created,
                CreatedAt = now,
                ExpiresAt = expire,
                Purpose = EnumPaymentPurpose.Deposit
            };
            payment.MarkPending();

            await _unitOfWork.RentalOrders.AddAsync(order);
            await _unitOfWork.Payments.AddAsync(payment);
            _unitOfWork.SaveChanges();

            var orderInfo = $"Giu cho thue xe - RentalOrder:{order.Id}";

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

            methodResult.Result = new CreateHoldResponse
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