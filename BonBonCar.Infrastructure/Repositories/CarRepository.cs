using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
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
            var vehicle = await _dbContext.Cars.FirstOrDefaultAsync(v => v.LicensePlate == licensePlateId);
            if (vehicle == null)
            {
                return null;
            }
            return vehicle;
        }
    }
}
