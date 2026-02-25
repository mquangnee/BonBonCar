namespace BonBonCar.Domain.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPaymentRepository Payments { get; }
        IRentalOrderRepository RentalOrders { get; }
        ICarImageRepository CarImages { get; }
        ICarRepository Cars { get; }
        IRegisterOtpSessionRepository RegisterOtpSessions { get; }
        IBrandRepository Brands { get; }
        IModelRepository Models { get; }
        IBasePriceRepository BasePrices { get; }
        ICarPriceRepository CarPrices { get; }
        IIdentityVerificationRepository IdentityVerification { get; }
        int SaveChanges();   
    }
}
