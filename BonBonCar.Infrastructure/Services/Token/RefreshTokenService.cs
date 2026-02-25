using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.ServiceModel.TokenService;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Services.Token
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly DataContext _dbContext;

        public RefreshTokenService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string refreshToken)
        {
            var hash = TokenUtil.Sha256Token(refreshToken);
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash && t.RevokedAt == null && t.ExpiresAt > DateTime.Now);
            return token;
        }

        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
        {
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
            if (token == null)
            {
                return null;
            }
            return token;
        }

        public async Task<string> IssueAsync(Guid userId)
        {
            var refreshToken = TokenUtil.GenerateRefeshToken();
            var hash = TokenUtil.Sha256Token(refreshToken);
            var token = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresAt = DateTime.Now.AddDays(7)
            };
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var hash = TokenUtil.Sha256Token(refreshToken);
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
            if (token == null) return;
            token.RevokedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync(Guid userId, string refreshToken)
        {
            var hash = TokenUtil.Sha256Token(refreshToken);
            var token = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                ExpiresAt = DateTime.Now.AddDays(7)
            };
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
