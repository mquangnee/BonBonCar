using BonBonCar.Domain.Entities;
using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository 
    {
        public PaymentRepository(DataContext dbContext) : base(dbContext) 
        {
        }
    }
}
