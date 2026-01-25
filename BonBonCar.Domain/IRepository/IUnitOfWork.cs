using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        int SaveChanges();   
    }
}
