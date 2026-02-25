using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Payment;

namespace BonBonCar.Domain.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetByTxnRefAsync(string txnRef, CancellationToken ct);
        Task<Payment?> GetLatestPendingByRentalOrderAsync(Guid rentalOrderId, EnumPaymentProvider provider, CancellationToken ct);
    }
}
