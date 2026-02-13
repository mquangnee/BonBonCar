using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Car
{
    public enum PaymentStatus
    {
        [Description("Chờ thanh toán")]
        Pending = 0,
        [Description("Thành công")]
        Success = 1,
        [Description("Thất bại")]
        Failed = 2,
        [Description("Đã hủy")]
        Cancelled = 3,
        [Description("Đã hoàn tiền")]
        Refunded = 4
    }
}
