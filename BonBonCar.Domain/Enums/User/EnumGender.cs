using System.ComponentModel;

namespace BonBonCar.Domain.Enums.User
{
    public enum EnumGender
    {
        [Description("Không xác định")]
        Unknown = 0,
        [Description("Nam")]
        Male = 1,
        [Description("Nữ")]
        Female = 2,
        [Description("Khác")]
        Other = 3
    }
}
