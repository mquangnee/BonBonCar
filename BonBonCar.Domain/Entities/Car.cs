using BonBonCar.Domain.Enums.Car;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class Car
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ModelId { get; set; }
        /// <summary>
        /// Location code of the car (e.g. HN, HCM,...).
        /// </summary>
        [StringLength(10)]
        public string? Location { get; set; }
        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }
        [Required]
        [StringLength(20)]
        public string? LicensePlate { get; set; }
        public string? PickupAddress { get; set; }
        [Required]
        public IList<string>? Features { get; set; }
        [Required]
        public EnumCarStatus Status { get; set; } = EnumCarStatus.Inactive;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
