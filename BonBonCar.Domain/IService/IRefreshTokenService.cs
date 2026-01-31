using BonBonCar.Domain.Entities;

namespace BonBonCar.Domain.IService
{
    public interface IRefreshTokenService
    {
        Task<string> IssueAsync(Guid userId);
        Task SaveAsync(Guid userId, string refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string refreshToken);
        Task RevokeAsync(string refreshToken);
        Task<RefreshToken> GetByUserIdAsync(Guid userId);
    }
}
