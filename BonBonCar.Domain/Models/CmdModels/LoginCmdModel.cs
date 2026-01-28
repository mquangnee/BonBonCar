using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels
{
    public class LoginCmdModel
    {
        [Required]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
