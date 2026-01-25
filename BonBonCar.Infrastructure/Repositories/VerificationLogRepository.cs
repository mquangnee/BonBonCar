using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class VerificationLogRepository : Repository<VerificationLog>, IVerificationLogRepository
    {
        public VerificationLogRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
