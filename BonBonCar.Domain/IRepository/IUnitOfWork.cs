namespace BonBonCar.Domain.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPaymentRepository Payments { get; }
        IRentalContractRepository RentalContracts { get; }
        IRentalOrderRepository RentalOrders { get; }
        IUserDocumentRepository UserDocuments { get; }
        ICarImageRepository VehicleImages { get; }
        ICarRepository Vehicles { get; }
        IVerificationLogRepository VerificationLogs { get; }
        IVerificationSessionRepository VerificationSessions { get; }
        IRegisterOtpSessionRepository RegisterOtpSessions { get; }
        IBrandRepository Brands { get; }
        IModelRepository Models { get; }
        IBasePriceRepository BasePrices { get; }
        int SaveChanges();   
    }
}
