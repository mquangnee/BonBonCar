using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.CmdModels
{
    public class RegisterCmdModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = default!;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = default!;
    }
}
