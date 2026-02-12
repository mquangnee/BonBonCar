using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class AuthModel
    {
        [Required]
        public string? AccessToken { get; set; }

        [Required]
        public string? RefreshToken { get; set; }
    }
}
