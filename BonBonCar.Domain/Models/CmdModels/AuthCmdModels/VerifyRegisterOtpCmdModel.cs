namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class VerifyRegisterOtpCmdModel
    {
        public Guid SessionId { get; set; }
        public string Otp { get; set; } = default!;
    }
}
