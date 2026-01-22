using BonBonCar.Domain.Enums.Verification;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class VerificationSession
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public VerificationType VerificationType { get; set; }
        [Required]
        public VerificationOverallStatus OverallStatus { get; set; } = VerificationOverallStatus.Pending;
        [Required]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public ICollection<VerificationLog> Logs { get; set; } = new List<VerificationLog>();
    }
}
