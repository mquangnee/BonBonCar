using BonBonCar.Domain.Enums.Car;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class CarPrice
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public EnumRentalDuration RentalDuration { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
