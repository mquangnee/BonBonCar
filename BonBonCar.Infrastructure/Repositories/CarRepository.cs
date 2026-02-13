using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class CarRepository : Repository<Car>, ICarRepository
    {
        public CarRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<Car> GetByLicensePlate(string licensePlateId)
        {
            var car = await _dbContext.Cars.FirstOrDefaultAsync(v => v.LicensePlate == licensePlateId);
            if (car == null)
            {
                return null;
            }
            return car;
        }
    }
}
