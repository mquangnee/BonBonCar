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

        public IVehicleImageRepository VehicleImages { get; private set; }

        public IVehicleRepository Vehicles { get; private set; }

        public IVerificationLogRepository VerificationLogs { get; private set; }

        public IVerificationSessionRepository VerificationSessions { get; private set; }

        public IRegisterOtpSessionRepository RegisterOtpSessions { get; private set; }

        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
            Payments = new PaymentRepository(_dbContext);
            RentalContracts = new RentalContractRepository(_dbContext);
            RentalOrders = new RentalOrderRepository(_dbContext);
            UserDocuments = new UserDocumentRepository(_dbContext);
            VehicleImages = new VehicleImageRepository(_dbContext);
            Vehicles = new VehicleRepository(_dbContext);
            VerificationLogs = new VerificationLogRepository(_dbContext);
            VerificationSessions = new VerificationSessionRepository(_dbContext);
            RegisterOtpSessions = new RegisterOtpSessionRepository(_dbContext);
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
