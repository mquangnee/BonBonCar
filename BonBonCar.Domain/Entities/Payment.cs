using BonBonCar.Domain.Enums.Payment;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid RentalOrderId { get; set; }
        [ForeignKey(nameof(RentalOrderId))]
        public RentalOrder RentalOrder { get; set; } = null!;
        [Required]
        public EnumPaymentProvider Provider { get; set; } = EnumPaymentProvider.Vnpay;
        [Required]
        public EnumPaymentStatus Status { get; set; } = EnumPaymentStatus.Created;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(64)]
        public string TxnRef { get; set; } = default!;
        [MaxLength(64)]
        public string? ProviderTransactionNo { get; set; }
        [MaxLength(16)]
        public string? ProviderResponseCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public EnumPaymentPurpose Purpose { get; set; }
        public string? RawIpn { get; set; }

        public void MarkPending()
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Pending;
        }

        public void MarkPaid(string transactionNo, string responseCode, string rawIpn)
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Paid;
            ProviderTransactionNo = transactionNo;
            ProviderResponseCode = responseCode;
            RawIpn = rawIpn;
            PaidAt = DateTime.Now;
        }

        public void MarkFailed(string responseCode, string rawIpn)
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Failed;
            ProviderResponseCode = responseCode;
            RawIpn = rawIpn;
        }

        public void MarkExpired()
        {
            if (Status == EnumPaymentStatus.Paid) return;
            Status = EnumPaymentStatus.Expired;
        }

        public void MarkRefunded(string responseCode, string raw)
        {
            Status = EnumPaymentStatus.Refunded;
            ProviderResponseCode = responseCode;
            RawIpn = raw;
        }
    }
}
