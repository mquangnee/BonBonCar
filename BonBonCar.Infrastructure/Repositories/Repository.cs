using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DataContext _dbContext;

        public Repository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T Add(T newEntity)
        {
            _dbContext.Set<T>().Add(newEntity);
            return newEntity;
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            return entity != null ? true : false;
        }

        public async Task<bool> DeleteAsync(T deleteEntity)
        {
            var entity = _dbContext.Set<T>().Remove(deleteEntity);
            return entity != null ? true : false;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public T Update(T updateEntity)
        {
            _dbContext.Update(updateEntity);
            return updateEntity;
        }
    }
}
