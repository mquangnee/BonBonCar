using BonBonCar.Domain.Models.ServiceModel.VnpayService;

namespace BonBonCar.Domain.IService
{
    public interface IVnpayTransactionService
    {
        Task<VnpayRefundResult> RefundAsync(VnpayRefundRequest request, CancellationToken ct);
    }
}

