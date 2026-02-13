using BonBonCar.Domain.Enums.Car;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class Model
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public EnumCarType VehicleType { get; set; }
        public bool IsActive { get; set; } = true;

        public Brand? Brand { get; set; }
    }
}
