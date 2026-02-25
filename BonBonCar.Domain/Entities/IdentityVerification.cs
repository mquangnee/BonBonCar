using BonBonCar.Domain.Enums.Verification;
using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Entities
{
    public class IdentityVerification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public EnumVerificationStatus Status { get; set; }

        public string? CccdFullName { get; set; }
        public DateTime? CccdDateOfBirth { get; set; }
        public string? CccdNationality { get; set; }
        public string? CccdPlaceOfResidence { get; set; }

        public DateTime? CccdVerifiedAt { get; set; }
        public DateTime? BlxVerifiedAt { get; set; }

        public string? LastRejectReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
