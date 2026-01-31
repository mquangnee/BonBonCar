namespace BonBonCar.Domain.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T newEntity);
        T Update(T updateEntity);
        bool DeleteAsync(T deleteEntity);
        Task<T?> GetByIdAsync(Guid id);
        Task<bool> AnyAsync(Guid id);
    }
}
