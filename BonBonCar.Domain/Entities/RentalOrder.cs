using BonBonCar.Domain.Enums.Car;
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
        public Guid RenterId { get; set; } // Identity UserId (không cần navigation tới ApplicationUser)

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public RentalOrderStatus Status { get; set; } = RentalOrderStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation (tuỳ chọn)
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; } = null!;

        // Nếu mỗi order chỉ có 1 contract
        public RentalContract? RentalContract { get; set; }

        // Thường payment có thể nhiều lần (cọc, phần còn lại, hoàn tiền...)
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
