namespace BonBonCar.Domain.IService
{
    public interface IOtpService
    {
        void SetOtpFlag(string Email);
        bool IsVerifiedOtp(string Email);
        void SetOtp(string Email, string OTP);
        string? GetOtp(string Email);
        void RemoveOtp(string Email);
        //void SetPassword(string Email, string Password);
        //string? GetPassword(string Email);
        //void RemovePassword(string Email);
    }
}
