using BonBonCar.Domain.Enums.Vehicle;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid RentalOrderId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        public PaymentType PaymentType { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        [Required]
        [StringLength(100)]
        public string? TransactionId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PaidAt { get; set; }
        [ForeignKey(nameof(RentalOrderId))]
        public RentalOrder? RentalOrder { get; set; }
    }
}
