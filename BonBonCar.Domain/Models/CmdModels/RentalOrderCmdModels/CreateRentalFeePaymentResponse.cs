namespace BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels
{
    public class CreateRentalFeePaymentResponse
    {
        public Guid RentalOrderId { get; set; }
        public Guid PaymentId { get; set; }
        public string TxnRef { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string PaymentUrl { get; set; } = "";
    }
}
