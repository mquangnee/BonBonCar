using BonBonCar.Domain.Enums.Payment;

namespace BonBonCar.Domain.Models.QueryModels.Payments
{
    public class PaymentStatus
    {
        public EnumPaymentStatus Status { get; set; }
        public string? ProviderTransactionNo { get; set; }
        public string? ProviderResponseCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
