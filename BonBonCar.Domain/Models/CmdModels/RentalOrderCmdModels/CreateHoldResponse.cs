namespace BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels
{
    public class CreateHoldResponse
    {
        public Guid RentalOrderId { get; set; }

        public Guid PaymentId { get; set; }

        public string TxnRef { get; set; } = default!;

        public decimal Amount { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string PaymentUrl { get; set; } = default!;

        public CreateHoldResponse() { }

        public CreateHoldResponse(
            Guid rentalOrderId,
            Guid paymentId,
            string txnRef,
            decimal amount,
            DateTime expiresAt,
            string paymentUrl)
        {
            RentalOrderId = rentalOrderId;
            PaymentId = paymentId;
            TxnRef = txnRef;
            Amount = amount;
            ExpiresAt = expiresAt;
            PaymentUrl = paymentUrl;
        }
    }
}
