using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class VehicleImage
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid VehicleId { get; set; }
        [Required]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        [Required]
        public bool IsPrimary { get; set; } = false;
    }
}
