using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<Vehicle> GetByLicensePlate(string licensePlateId)
        {
            var vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate == licensePlateId);
            if (vehicle == null)
            {
                return null;
            }
            return vehicle;
        }
    }
}
