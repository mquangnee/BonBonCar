namespace BonBonCar.Domain.IService
{
    public interface IEmailTemplate
    {
        Task<string> GetOtpEmailBodyAsync(string otp, string title, string previewLine, string messageLine, string userName, CancellationToken cancellationToken);
    }
}
