using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Models.EntityModels;

namespace BonBonCar.Domain.IRepository
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<Vehicle> GetByLicensePlate(string licensePlateId);
    }
}
