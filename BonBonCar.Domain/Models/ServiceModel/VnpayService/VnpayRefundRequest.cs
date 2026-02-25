namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public sealed class VnpayRefundRequest
    {
        public string TxnRef { get; set; } = default!;
        public decimal AmountVnd { get; set; }

        /// <summary>
        /// Transaction type: 02 full refund, 03 partial refund
        /// </summary>
        public string TransactionType { get; set; } = "02";

        /// <summary>
        /// Merchant transaction date (yyyyMMddHHmmss, GMT+7), usually equals vnp_CreateDate of pay request.
        /// </summary>
        public string TransactionDate { get; set; } = default!;

        /// <summary>
        /// VNPay transaction no (vnp_TransactionNo) - optional but recommended if available.
        /// </summary>
        public string? TransactionNo { get; set; }

        public string OrderInfo { get; set; } = default!;
        public string CreateBy { get; set; } = "system";
        public string CreateDate { get; set; } = default!;
        public string IpAddr { get; set; } = "127.0.0.1";

        /// <summary>
        /// Unique request id within day (<=32 chars).
        /// </summary>
        public string RequestId { get; set; } = default!;
    }
}

