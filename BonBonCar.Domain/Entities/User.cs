using BonBonCar.Domain.Enums.User;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class User
    {
        [Required]  
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }
        [Required]
        public EnumGender Gender { get; set; } = EnumGender.Unknown;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        [StringLength(300)]
        public string? Address { get; set; }
        [Required]
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
