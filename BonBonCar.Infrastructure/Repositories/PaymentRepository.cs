using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository 
    {
        public PaymentRepository(DataContext dbContext) : base(dbContext) 
        {
        }

        public async Task<Payment?> GetByTxnRefAsync(string txnRef, CancellationToken ct)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.TxnRef == txnRef);
            return payment;
        }

        public async Task<Payment?> GetLatestPendingByRentalOrderAsync(Guid rentalOrderId, EnumPaymentProvider provider, CancellationToken ct)
        {
            var pendingStatuses = new[]
            {
                EnumPaymentStatus.Created,
                EnumPaymentStatus.Pending
            };

            var now = DateTime.Now;

            return await _dbContext.Payments.AsNoTracking()
                .Where(x => x.RentalOrderId == rentalOrderId
                         && x.Provider == provider
                         && pendingStatuses.Contains(x.Status))
                .Where(x => x.ExpiresAt == null || x.ExpiresAt > now)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }
    }
}
