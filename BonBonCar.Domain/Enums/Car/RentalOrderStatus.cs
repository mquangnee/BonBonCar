using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Vehicle
{
    public enum RentalOrderStatus
    {
        [Description("Chờ xác nhận")]
        Pending = 0,
        [Description("Đã xác nhận")]
        Confirmed = 1,
        [Description("Đang thuê")]
        Active = 2,
        [Description("Hoàn tất")]
        Completed = 3,
        [Description("Đã huỷ")]
        Cancelled = 4
    }
}
