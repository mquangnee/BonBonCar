using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class ForgotPasswordCmdModel
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Otp { get; set; }
        public string? NewPassword { get; set; }
        [Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; }
    }
}
