using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; } = default!;

        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
