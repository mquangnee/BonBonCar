using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Vehicle
{
    public enum RentalContractStatus
    {
        [Description("Chờ ký")]
        Pending = 0,
        [Description("Đã ký")]
        Signed = 1,
        [Description("Đã hủy")]
        Cancelled = 2
    }
}
