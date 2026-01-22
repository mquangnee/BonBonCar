using BonBonCar.Domain.Enums.Verification;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class VerificationLog
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid VerificationSessionId { get; set; }
        [Required]
        public VerificationStep Step { get; set; }
        [Required]
        public bool IsPassed { get; set; }
        [Range(0, 100)]
        public int MatchScore { get; set; }
        [Required]
        [StringLength(500)]
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
