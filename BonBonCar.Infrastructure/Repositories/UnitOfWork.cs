using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;
        public IPaymentRepository Payments { get; private set; }

        public IRentalOrderRepository RentalOrders { get; private set; }

        public ICarImageRepository CarImages { get; private set; }

        public ICarRepository Cars { get; private set; }

        public IRegisterOtpSessionRepository RegisterOtpSessions { get; private set; }

        public IBrandRepository Brands { get; private set; }
        
        public IModelRepository Models { get; private set; }
        
        public IBasePriceRepository BasePrices { get; private set; }

        public ICarPriceRepository CarPrices { get; private set; }
       
        public IIdentityVerificationRepository IdentityVerification { get; private set; }

        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
            Payments = new PaymentRepository(_dbContext);
            RentalOrders = new RentalOrderRepository(_dbContext);
            CarImages = new CarImageRepository(_dbContext);
            Cars = new CarRepository(_dbContext);
            RegisterOtpSessions = new RegisterOtpSessionRepository(_dbContext);
            Brands = new BrandRepository(_dbContext);
            Models = new ModelRepository(_dbContext);
            BasePrices = new BasePriceRepository(_dbContext);
            CarPrices = new CarPriceRepository(_dbContext);
            IdentityVerification = new IdentityVerificationRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
