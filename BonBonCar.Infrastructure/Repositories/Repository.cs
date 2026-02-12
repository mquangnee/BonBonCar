using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DataContext _dbContext;

        public Repository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T newEntity)
        {
            await _dbContext.Set<T>().AddAsync(newEntity);
            return newEntity;
        }

        public async Task<bool> AnyAsync(Guid id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            return entity != null ? true : false;
        }

        public bool DeleteAsync(T deleteEntity)
        {
            var entity = _dbContext.Set<T>().Remove(deleteEntity);
            return entity != null ? true : false;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> QueryableAsync()
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public T Update(T updateEntity)
        {
            _dbContext.Update(updateEntity);
            return updateEntity;
        }
    }
}
