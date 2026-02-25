namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public class VnpayBuildUrlRequest
    {
        public string TxnRef { get; set; } = default!;
        public decimal AmountVnd { get; set; }
        public string OrderInfo { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public DateTime CreateDateLocal { get; set; }
        public DateTime ExpireDateLocal { get; set; }
        public string ReturnUrl { get; set; } = default!;
        public string? IpnUrl { get; set; }

        public VnpayBuildUrlRequest() { }

        public VnpayBuildUrlRequest(
            string txnRef,
            decimal amountVnd,
            string orderInfo,
            string ipAddress,
            DateTime createDateLocal,
            DateTime expireDateLocal,
            string returnUrl,
            string ipnUrl)
        {
            TxnRef = txnRef;
            AmountVnd = amountVnd;
            OrderInfo = orderInfo;
            IpAddress = ipAddress;
            CreateDateLocal = createDateLocal;
            ExpireDateLocal = expireDateLocal;
            ReturnUrl = returnUrl;
            IpnUrl = ipnUrl;
        }
    }
}
