using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository
    {
        public UserDocumentRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
