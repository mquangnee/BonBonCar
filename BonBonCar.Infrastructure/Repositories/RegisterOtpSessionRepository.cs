using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class RegisterOtpSessionRepository : Repository<RegisterOtpSession>, IRegisterOtpSessionRepository
    {
        public RegisterOtpSessionRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
