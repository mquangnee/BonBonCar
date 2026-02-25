namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public class VnpayOptions
    {
        public string TmnCode { get; set; } = default!;
        public string HashSecret { get; set; } = default!;
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Version { get; set; } = "2.1.0";
        public string Locale { get; set; } = "vn";
        public string PaymentUrl { get; set; } = default!;
        public string ReturnUrl { get; set; } = default!;

        // ✅ để trống khi dev local chưa public
        public string? IpnUrl { get; set; }

        // API querydr/refund
        public string TransactionApiUrl { get; set; } = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        public string RefundCreateBy { get; set; } = "system";
    }
}
