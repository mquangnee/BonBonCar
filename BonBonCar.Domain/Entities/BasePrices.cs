using BonBonCar.Domain.Enums.Car;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BonBonCar.Domain.Entities
{
    public class BasePrices
    {
        [Key]   
        public Guid Id { get; set; }
        public EnumCarType CarType { get; set; }
        public EnumRentalDuration RentalDuration { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice {  get; set; } 
        public bool IsActive { get; set; } = true;
    }
}
