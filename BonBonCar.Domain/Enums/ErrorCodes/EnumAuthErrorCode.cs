namespace BonBonCar.Domain.Enums.ErrorCodes
{
    public enum EnumAuthErrorCode
    {
        InvalidCredentials,
        RegisterSessionNotExist,
        RegisterSessionUsed,
        OtpExpired,
        EnterOtpTooMuch,
        OtpNotValid,
        ChangePasswordFailed,
        AccountLocked,
        TokenExpired,
        TokenInvalid,
        Unauthorized, 
        SessionExpired,
        RegistrationFailed
    }
}
