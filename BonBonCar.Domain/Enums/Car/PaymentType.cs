using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Vehicle
{
    public enum PaymentType
    {
        [Description("Đặt cọc")]
        Deposit = 0,
        [Description("Thanh toán")]
        RentalFee = 1,
        [Description("Hoàn tiền")]
        Refund = 2
    }
}
