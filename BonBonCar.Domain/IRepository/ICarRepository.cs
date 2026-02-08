using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Models.EntityModels;

namespace BonBonCar.Domain.IRepository
{
    public interface ICarRepository : IRepository<Car>
    {
        Task<Car> GetByLicensePlate(string licensePlateId);
    }
}
