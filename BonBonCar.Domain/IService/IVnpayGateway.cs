using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.IService
{

    public interface IVnpayGateway
    {
        string BuildPaymentUrl(VnpayBuildUrlRequest req);
        VnpayValidateResult ValidateCallback(IQueryCollection query);
    }
}
