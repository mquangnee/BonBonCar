using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class ModelRepository : Repository<Model>, IModelRepository
    {
        public ModelRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<Model>> GetByBrandIdAsync(Guid BrandId)
        {
            return await _dbContext.Set<Model>().Where(m => m.BrandId == BrandId).ToListAsync();
        }
    }
}
