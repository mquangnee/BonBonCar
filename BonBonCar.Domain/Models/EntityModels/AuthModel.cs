using System.ComponentModel.DataAnnotations;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class AuthModel
    {
        [Required]
        public string? AccessToken { get; set; }

        [Required]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Vai trò chính của người dùng (ví dụ: "Admin" hoặc "User").
        /// FE có thể dùng field này để điều hướng sang trang admin sau khi đăng nhập.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
