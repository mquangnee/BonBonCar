namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public sealed class VnpayRefundResult
    {
        public bool IsSuccess { get; set; }
        public bool IsValidSignature { get; set; }
        public string ResponseCode { get; set; } = "99";
        public string Message { get; set; } = "Unknown";
        public string? TransactionStatus { get; set; }
        public string Raw { get; set; } = string.Empty;
    }
}

