namespace BonBonCar.Domain.Models.ServiceModel.VnpayService
{
    public class VnpayIpnResponse
    {
        public string RspCode { get; set; } = "99";
        public string Message { get; set; } = "Unknown";

        public VnpayIpnResponse() { }
        public VnpayIpnResponse(string rspCode, string message)
        {
            RspCode = rspCode;
            Message = message;
        }
    }
}
