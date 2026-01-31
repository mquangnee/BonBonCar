namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class VerifyRegisterOtpCmdModel
    {
        public string SessionId { get; set; } = default!;
        public string Otp { get; set; } = default!;
    }
}
