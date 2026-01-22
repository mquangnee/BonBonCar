using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Vehicle
{
    public enum PaymentMethod
    {
        [Description("Tiền mặt")]
        Cash = 0,
        [Description("Chuyển khoản ngân hàng")]
        BankTransfer = 1,
        [Description("Ví điện tử")]
        EWallet = 2,
        [Description("Thẻ")]
        Card = 3
    }
}
