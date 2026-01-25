using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonBonCar.Domain.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Add(T newEntity);
        T Update(T updateEntity);
        Task<bool> DeleteAsync(T deleteEntity);
        Task<T> GetByIdAsync(Guid id);
        Task<bool> AnyAsync(Guid id);
    }
}
