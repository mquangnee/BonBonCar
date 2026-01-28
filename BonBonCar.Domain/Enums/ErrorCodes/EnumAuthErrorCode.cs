namespace BonBonCar.Domain.Enums.ErrorCodes
{
    public enum EnumAuthErrorCode
    {
        InvalidCredentials,    // Sai tên đăng nhập hoặc mật khẩu
        RegisterSessionNotExist, // Phiên đăng ký không tồn tại
        RegisterSessionUsed,   // Phiên đăng ký đã được sử dụng
        OtpExpired,            // OTP đã hết hạn
        EnterOtpTooMuch,       // Nhập OTP quá số lần cho phép
        OtpNotValid,         // OTP không hợp lệ

        AccountLocked,         // Tài khoản bị khóa
        TokenExpired,          // Token hết hạn
        TokenInvalid,          // Token không hợp lệ
        Unauthorized,          // Không có quyền truy cập
        SessionExpired,        // Phiên đăng nhập đã hết hạn
        RegistrationFailed     // Đăng ký thất bại
    }
}
