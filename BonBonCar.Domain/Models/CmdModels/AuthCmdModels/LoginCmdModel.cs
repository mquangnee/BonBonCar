using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels.AuthCmdModels
{
    public class LoginCmdModel
    {
        [Required]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
