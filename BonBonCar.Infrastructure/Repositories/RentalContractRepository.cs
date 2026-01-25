using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class RentalContractRepository : Repository<RentalContract>, IRentalContractRepository
    {
        public RentalContractRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
