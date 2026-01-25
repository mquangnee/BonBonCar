using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class VerificationSessionRepository : Repository<VerificationSession>, IVerificationSessionRepository
    {
        public VerificationSessionRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
