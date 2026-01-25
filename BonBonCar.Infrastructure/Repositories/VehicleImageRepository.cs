using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class VehicleImageRepository : Repository<VehicleImage>, IVehicleImageRepository
    {
        public VehicleImageRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
