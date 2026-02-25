using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.ServiceModel.GoogleService;
using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace BonBonCar.Infrastructure.Services.GoogleDocumentAI
{
    public sealed class DocumentAiService : IDocumentAiService
    {
        private readonly GoogleDocumentAiOptions _googleDocumentAiOptions;

        public DocumentAiService(IOptions<GoogleDocumentAiOptions> option)
        {
            _googleDocumentAiOptions = option.Value;
        }

        public async Task<CccdExtractResult> ExtractCccdAsync(byte[] imageBytes, string mimeType, CancellationToken ct)
        {
            var client = await DocumentProcessorServiceClient.CreateAsync();

            string name = BuildProcessorOrVersionName(_googleDocumentAiOptions.CccdProcessorId ?? string.Empty, _googleDocumentAiOptions.CccdProcessorVersionId ?? string.Empty);

            var req = new ProcessRequest
            {
                Name = name,
                InlineDocument = new Document
                {
                    Content = ByteString.CopyFrom(imageBytes),
                    MimeType = mimeType
                }
            };

            var resp = await client.ProcessDocumentAsync(req, cancellationToken: ct);
            
            return MapCccd(resp.Document);
        }

        public async Task<BlxExtractResult> ExtractBlxAsync(byte[] imageBytes, string mimeType, CancellationToken ct)
        {
            var client = await DocumentProcessorServiceClient.CreateAsync();
            string name = BuildProcessorOrVersionName(_googleDocumentAiOptions.BlxProcessorId ?? string.Empty, _googleDocumentAiOptions.BlxProcessorVersionId ?? string.Empty);

            var req = new ProcessRequest
            {
                Name = name,
                InlineDocument = new Document
                {
                    Content = ByteString.CopyFrom(imageBytes),
                    MimeType = mimeType
                }
            };

            var resp = await client.ProcessDocumentAsync(req, cancellationToken: ct);

            return MapBlx(resp.Document);
        }

        private string BuildProcessorOrVersionName(string processorId, string versionId)
        {
            if (!string.IsNullOrWhiteSpace(versionId))
            {
                var v = ProcessorVersionName.FromProjectLocationProcessorProcessorVersion(
                    _googleDocumentAiOptions.ProjectId, _googleDocumentAiOptions.Location, processorId, versionId);
                return v.ToString();
            }

            var p = ProcessorName.FromProjectLocationProcessor(_googleDocumentAiOptions.ProjectId, _googleDocumentAiOptions.Location, processorId);
            return p.ToString();
        }

        private static CccdExtractResult MapCccd(Document doc)
        {
            var result = new CccdExtractResult();

            foreach (var e in doc.Entities)
            {
                var key = (e.Type ?? "").Trim().ToLowerInvariant();
                var val = e.MentionText?.Trim();
                var conf = e.Confidence;

                switch (key)
                {
                    case "document_number":
                        result.DocumentNumber = KeepDigits(val);
                        break;

                    case "full_name":
                        result.FullName = val;
                        break;

                    case "gender":
                        result.Gender = val;
                        break;

                    case "nationality":
                        result.Nationality = val;
                        break;

                    case "place_of_origin":
                        result.PlaceOfOrigin = val;
                        break;

                    case "place_of_residence":
                        result.PlaceOfResidence = val;
                        break;

                    case "date_of_birth":
                        result.DateOfBirth = ParseDate(val);
                        break;

                    case "expiry_date":
                        result.ExpiryDate = ParseDate(val);
                        break;
                }
            }

            return result;
        }

        private static BlxExtractResult MapBlx(Document doc)
        {
            var result = new BlxExtractResult();

            foreach (var e in doc.Entities)
            {
                var key = (e.Type ?? "").Trim().ToLowerInvariant();
                var val = e.MentionText?.Trim();

                switch (key)
                {
                    case "license_number":
                        result.LicenseNumber = KeepDigits(val);
                        break;
                    case "full_name":
                        result.FullName = val;
                        break;
                    case "date_of_birth":
                        result.DateOfBirth = ParseDate(val);
                        break;
                    case "nationality":
                        result.Nationality = val;
                        break;
                    case "residential_address":
                        result.ResidentialAddress = val;
                        break;
                    case "license_class":
                        result.LicenseClass = val;
                        break;
                    case "expiry_date":
                        result.ExpiryRaw = val;
                        result.ExpiryDate = ParseDate(val);
                        break;
                }
            }

            return result;
        }
        private static string? KeepDigits(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var digits = new string(s.Where(char.IsDigit).ToArray());
            return string.IsNullOrWhiteSpace(digits) ? null : digits;
        }

        private static DateTime? ParseDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            if (DateTime.TryParseExact(s.Trim(),
                    new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var d))
                return d.Date;

            if (DateTime.TryParse(s, out var d2)) return d2.Date;
            return null;
        }
    }
}
