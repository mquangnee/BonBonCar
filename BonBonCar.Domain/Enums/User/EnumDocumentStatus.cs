using System.ComponentModel;

namespace BonBonCar.Domain.Enums.User
{
    public enum EnumDocumentStatus
    {
        [Description("Chờ xử lý")]
        Pending = 0,
        [Description("Đã được chấp thuận")]
        Approved = 1,
        [Description("Bị từ chối")]
        Rejected = 2,
        [Description("Đã hết hạn")]
        Expired = 3
    }
}
