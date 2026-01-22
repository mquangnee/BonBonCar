using System.ComponentModel;

namespace BonBonCar.Domain.Enums.User
{
    public enum EnumDocumentType
    {
        [Description("Căn cước công dân")]
        CCCD = 0,
        [Description("Giấy phép lái xe")]
        DriverLicense = 1
    }
}
