using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class CarPriceRepository : Repository<CarPrice>, ICarPriceRepository
    {
        public CarPriceRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
