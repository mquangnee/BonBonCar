using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BonBonCar.Domain.Enums.Vehicle;

namespace BonBonCar.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid OwnerId { get; set; }
        [Required]
        [StringLength(50)]
        public string? Brand { get; set; }
        [Required]
        [StringLength(50)]
        public string? Model { get; set; }
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        [Required]
        [StringLength(20)]
        public string? LicensePlate { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerDay { get; set; }
        [Required]
        public EnumVehicleStatus Status { get; set; } = EnumVehicleStatus.Inactive;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
