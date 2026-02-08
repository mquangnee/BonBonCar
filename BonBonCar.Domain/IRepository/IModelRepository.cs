using BonBonCar.Domain.Entities;

namespace BonBonCar.Domain.IRepository
{
    public interface IModelRepository : IRepository<Model>
    {
        Task<IList<Model>> GetByBrandIdAsync(Guid BrandId);
    }
}
