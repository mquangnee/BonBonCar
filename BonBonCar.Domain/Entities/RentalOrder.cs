using BonBonCar.Domain.Enums.RentalOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class RentalOrder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CarId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public DateTime PickupDateTime { get; set; }
        public decimal TotalRentalFee { get; set; }
        [Required]
        public DateTime ReturnDateTime { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        public EnumRentalOrderStatus Status { get; set; } = EnumRentalOrderStatus.Created;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public void MarkHoldPending() => Status = EnumRentalOrderStatus.HoldPending;
        public void MarkHeld() => Status = EnumRentalOrderStatus.Held;
        public void MarkHoldFailed() => Status = EnumRentalOrderStatus.HoldFailed;
        public void MarkHoldExpired() => Status = EnumRentalOrderStatus.HoldExpired;
    }
}
