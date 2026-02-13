using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class CarImage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid CarId { get; set; }
        [Required]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        [Required]
        public bool IsPrimary { get; set; } = false;
    }
}
