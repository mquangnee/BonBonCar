using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Verification
{
    public enum VerificationStep
    {
        [Description("Nộp giấy tờ")]
        SubmitDocument = 0,
        [Description("Quét OCR")]
        OcrScan = 1,
        [Description("Đối chiếu thông tin")]
        DataComparison = 2,
        [Description("Hoàn tất")]
        Completed = 3
    }
}
