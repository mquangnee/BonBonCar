using System.Security.Cryptography;
using System.Text;

namespace BonBonCar.Domain.Models.ServiceModel.TokenService
{
    public static class TokenUtil
    {
        /// <summary>
        /// Tạo Refresh Token
        /// </summary>
        public static string GenerateRefeshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Hash Refresh Token
        /// </summary>
        public static string Sha256Token(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
