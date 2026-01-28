namespace BonBonCar.Domain.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPaymentRepository Payments { get; }
        IRentalContractRepository RentalContracts { get; }
        IRentalOrderRepository RentalOrders { get; }
        IUserDocumentRepository UserDocuments { get; }
        IVehicleImageRepository VehicleImages { get; }
        IVehicleRepository Vehicles { get; }
        IVerificationLogRepository VerificationLogs { get; }
        IVerificationSessionRepository VerificationSessions { get; }
        IRegisterOtpSessionRepository RegisterOtpSessions { get; }
        int SaveChanges();   
    }
}
