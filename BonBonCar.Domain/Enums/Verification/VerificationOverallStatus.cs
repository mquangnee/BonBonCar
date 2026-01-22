using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Verification
{
    public enum VerificationOverallStatus
    {
        [Description("Chờ xử lý")]
        Pending = 0,
        [Description("Đã xác minh thành công")]
        Approved = 1,
        [Description("Xác minh thất bại")]
        Rejected = 2
    }
}
