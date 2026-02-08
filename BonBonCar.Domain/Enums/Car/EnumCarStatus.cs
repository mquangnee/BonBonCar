using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Car
{
    public enum EnumCarStatus
    {
        [Description("Không hoạt động")]
        Inactive = 0,
        [Description("Sẵn sàng cho thuê")]
        Available = 1,
        [Description("Đang được thuê")]
        Rented = 2,
        [Description("Đang bảo trì")]
        Maintenance = 3,
        [Description("Bị vô hiệu hoá")]
        Disabled = 4
    }
}
