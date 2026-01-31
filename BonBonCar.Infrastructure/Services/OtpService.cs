using BonBonCar.Domain.IService;
using Microsoft.Extensions.Caching.Memory;

namespace BonBonCar.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _memoryCache;

        public OtpService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetOtpFlag(string Email)
        {
            _memoryCache.Set(Email, true, TimeSpan.FromMinutes(5));
        }

        public bool IsVerifiedOtp(string Email)
        {
            return _memoryCache.Get<bool>(Email);
        }

        public void SetOtp(string Email, string OTP)
        {
            _memoryCache.Set($"{Email}_otp", OTP, TimeSpan.FromMinutes(5));
        }

        public string? GetOtp(string Email)
        {
            return _memoryCache.Get<string>($"{Email}_otp");
        }

        public void RemoveOtp(string Email)
        {
            _memoryCache.Remove($"{Email}_otp");
        }

        //public void SetPassword(string Email, string Password)
        //{
        //    _memoryCache.Set($"{Email}_password", Password, TimeSpan.FromMinutes(10));
        //}

        //public string? GetPassword(string Email)
        //{
        //    return _memoryCache.Get<string>($"{Email}_password");
        //}

        //public void RemovePassword(string Email)
        //{
        //    _memoryCache.Remove($"{Email}_password");
        //}
    }
}
