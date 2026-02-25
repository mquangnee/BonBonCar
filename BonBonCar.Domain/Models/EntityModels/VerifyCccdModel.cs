using BonBonCar.Domain.Enums.Verification;
using BonBonCar.Domain.Models.ServiceModel.GoogleService;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class VerifyCccdModel
    {
        public EnumVerificationStatus Status { get; set; } = EnumVerificationStatus.DRAFT;
        public double Score { get; set; }
        public List<string> Reasons { get; set; } = new();
        public CccdExtractResult Extracted { get; set; } = new();
    }
}
