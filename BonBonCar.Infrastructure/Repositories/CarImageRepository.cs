using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class CarImageRepository : Repository<CarImage>, ICarImageRepository
    {
        public CarImageRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
