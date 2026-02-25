using BonBonCar.Domain.Entities;

namespace BonBonCar.Domain.IRepository
{
    public interface IRentalOrderRepository : IRepository<RentalOrder>
    {
        Task<IList<RentalOrder>> GetMyActiveAsync(Guid customerId, CancellationToken ct);
    }
}
