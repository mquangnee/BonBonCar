using BonBonCar.Domain.Models.ServiceModel.GoogleService;

namespace BonBonCar.Domain.IService
{
    public interface IDocumentAiService
    {
        Task<CccdExtractResult> ExtractCccdAsync(byte[] imageBytes, string mimeType, CancellationToken ct);
        Task<BlxExtractResult> ExtractBlxAsync(byte[] imageBytes, string mimeType, CancellationToken ct);
    }
}
