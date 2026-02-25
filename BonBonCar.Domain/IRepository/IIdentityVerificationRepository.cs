using BonBonCar.Domain.Entities;

namespace BonBonCar.Domain.IRepository
{
    public interface IIdentityVerificationRepository : IRepository<IdentityVerification>
    {
        Task<IdentityVerification?> GetByUserIdAsync(Guid userId);
    }
}
