using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class BasePriceRepository : Repository<BasePrices>, IBasePriceRepository
    {
        public BasePriceRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<BasePrices>> GetBasePriceByCarTypeAsync(EnumCarType carType)
        {
            return await _dbContext.BasePrices.Where(p => p.VehicleType == carType).ToListAsync();
        }
    }
}
