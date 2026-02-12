using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;

namespace BonBonCar.Domain.IRepository
{
    public interface IBasePriceRepository : IRepository<BasePrices>
    {
        Task<IList<BasePrices>> GetBasePriceByCarTypeAsync(EnumCarType carType);
    }
}
