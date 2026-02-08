using BonBonCar.Domain.IRepository;
using BonBonCar.Infrastructure.Persistence;

namespace BonBonCar.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;
        public IPaymentRepository Payments { get; private set; }

        public IRentalContractRepository RentalContracts { get; private set; }

        public IRentalOrderRepository RentalOrders { get; private set; }

        public IUserDocumentRepository UserDocuments { get; private set; }

        public ICarImageRepository VehicleImages { get; private set; }

        public ICarRepository Vehicles { get; private set; }

        public IVerificationLogRepository VerificationLogs { get; private set; }

        public IVerificationSessionRepository VerificationSessions { get; private set; }

        public IRegisterOtpSessionRepository RegisterOtpSessions { get; private set; }

        public IBrandRepository Brands { get; private set; }
        
        public IModelRepository Models { get; private set; }
        
        public IBasePriceRepository BasePrices { get; private set; }

        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
            Payments = new PaymentRepository(_dbContext);
            RentalContracts = new RentalContractRepository(_dbContext);
            RentalOrders = new RentalOrderRepository(_dbContext);
            UserDocuments = new UserDocumentRepository(_dbContext);
            VehicleImages = new CarImageRepository(_dbContext);
            Vehicles = new CarRepository(_dbContext);
            VerificationLogs = new VerificationLogRepository(_dbContext);
            VerificationSessions = new VerificationSessionRepository(_dbContext);
            RegisterOtpSessions = new RegisterOtpSessionRepository(_dbContext);
            Brands = new BrandRepository(_dbContext);
            Models = new ModelRepository(_dbContext);
            BasePrices = new BasePriceRepository(_dbContext);
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
