using BonBonCar.Domain.Enums.Verification;

namespace BonBonCar.Domain.Models.ServiceModel.GoogleService
{
    public class VerifyStepResult<TExtracted>
    {
        public EnumVerifyStepStatus Status { get; set; } = EnumVerifyStepStatus.REVIEW;
        public double Score { get; set; }
        public List<string> Reasons { get; set; } = new();
        public TExtracted Extracted { get; set; } = default!;
    }
}
