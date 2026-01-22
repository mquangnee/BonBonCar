using System.ComponentModel;

namespace BonBonCar.Domain.Enums.Verification
{
    public enum VerificationType
    {
        [Description("Xác minh khi đăng ký tài khoản")]
        Register = 0,
        [Description("Xác minh trước khi thuê xe")]
        RentCar = 1,
        [Description("Xác minh lại thông tin người dùng")]
        ReVerify = 2
    }
}
