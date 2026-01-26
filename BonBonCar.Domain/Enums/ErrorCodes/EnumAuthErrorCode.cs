namespace BonBonCar.Domain.Enums.ErrorCodes
{
    public enum EnumAuthErrorCode
    {
        InvalidCredentials,    // Sai tên đăng nhập hoặc mật khẩu
        UserNotFound  ,        // Không tìm thấy người dùng
        AccountLocked,         // Tài khoản bị khóa
        TokenExpired,          // Token hết hạn
        TokenInvalid,          // Token không hợp lệ
        Unauthorized,          // Không có quyền truy cập
        SessionExpired,        // Phiên đăng nhập đã hết hạn
        RegistrationFailed     // Đăng ký thất bại
    }
}
