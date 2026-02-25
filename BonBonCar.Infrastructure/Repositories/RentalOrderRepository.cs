using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.RentalOrder;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class RentalOrderRepository : Repository<RentalOrder>, IRentalOrderRepository
    {
        public RentalOrderRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<RentalOrder>> GetMyActiveAsync(Guid customerId, CancellationToken ct)
        {
            var activeStatuses = new[]
            {
                EnumRentalOrderStatus.Created,
                EnumRentalOrderStatus.HoldPending,
                EnumRentalOrderStatus.Held
            };

            return await _dbContext.RentalOrders
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId && activeStatuses.Contains(x.Status))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}