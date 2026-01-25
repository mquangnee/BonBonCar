using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class RentalOrderRepository : Repository<RentalOrder>, IRentalOrderRepository
    {
        public RentalOrderRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
