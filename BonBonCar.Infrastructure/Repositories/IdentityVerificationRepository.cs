using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class IdentityVerificationRepository : Repository<IdentityVerification>, IIdentityVerificationRepository
    {
        public IdentityVerificationRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IdentityVerification?> GetByUserIdAsync(Guid userId)
        {
            var identityVerification = await _dbContext.IdentityVerifications.FirstOrDefaultAsync(i =>  i.UserId == userId);
            if (identityVerification == null)
            {
                return null;
            }
            return identityVerification;
        }
    }
}
