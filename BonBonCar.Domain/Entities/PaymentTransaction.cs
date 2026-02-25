using BonBonCar.Domain.Enums.Payment;

namespace BonBonCar.Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid OrderId { get; private set; }
        public long AmountVnd { get; private set; }
        public EnumPaymentStatus Status { get; private set; } = EnumPaymentStatus.Created;
        public string TxnRef { get; private set; } = default!;
        public string? VnpTransactionNo { get; private set; }
        public string? LastVnpResponseCode { get; private set; }
        public string? RawIpn { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.Now;
        public DateTimeOffset? PaidAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }

        private PaymentTransaction() { }
        public PaymentTransaction(Guid orderId, long amountVnd, string txnRef, DateTimeOffset expiresAt)
        {
            OrderId = orderId;
            AmountVnd = amountVnd;
            TxnRef = txnRef;
            ExpiresAt = expiresAt;
            Status = EnumPaymentStatus.Created;
        }

        public void MarkPending()
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Pending;
        }

        public void MarkPaid(string vnpTransactionNo, string responseCode, string rawIpn, DateTimeOffset paidAt)
        {
            if (Status == EnumPaymentStatus.Paid) return; // idempotent
            Status = EnumPaymentStatus.Paid;
            VnpTransactionNo = vnpTransactionNo;
            LastVnpResponseCode = responseCode;
            RawIpn = rawIpn;
            PaidAt = paidAt;
        }

        public void MarkFailed(string responseCode, string rawIpn)
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Failed;
            LastVnpResponseCode = responseCode;
            RawIpn = rawIpn;
        }

        public void MarkExpired()
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Expired;
        }
    }
}
