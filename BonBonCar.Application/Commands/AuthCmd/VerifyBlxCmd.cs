using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.Enums.Verification;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.AuthCmdModels;
using BonBonCar.Domain.Models.ServiceModel.GoogleService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class VerifyBlxCmd : VerifyBlxCmdModel, IRequest<MethodResult<bool>>
    {
        public byte[]? FrontImageBytes { get; set; }
        public string? FrontImageMimeType { get; set; }
    }

    public class VerifyBlxCmdHandler : IRequestHandler<VerifyBlxCmd, MethodResult<bool>>
    {
        private const double W_FULLNAME = 0.45;
        private const double W_DOB = 0.35;
        private const double W_NATIONALITY = 0.10;
        private const double W_PERM_ADDR = 0.10;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentAiService _documentAiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public VerifyBlxCmdHandler(IUnitOfWork unitOfWork, IDocumentAiService documentAiService, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _documentAiService = documentAiService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<MethodResult<bool>> Handle(VerifyBlxCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            if (request.FrontImageBytes == null || request.FrontImageMimeType == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.InValidFormat));
                return methodResult;
            }

            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }
            var user = await _userManager.FindByIdAsync(userIdStr);
            if (user == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }

            var identityVerification = await _unitOfWork.IdentityVerification.GetByUserIdAsync(userId);
            if (identityVerification == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            if (identityVerification.Status != EnumVerificationStatus.CCCDPASSED)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.InValidFormat));
                return methodResult;
            }

            var extracted = await _documentAiService.ExtractBlxAsync(request.FrontImageBytes, request.FrontImageMimeType, cancellationToken);
            var (score, reasons) = ScoreBlx(identityVerification, extracted);
            var stepStatus = score >= 0.85 ? EnumVerifyStepStatus.PASS : EnumVerifyStepStatus.FAIL;
            if(stepStatus == EnumVerifyStepStatus.PASS)
            {
                identityVerification.Status = EnumVerificationStatus.VERIFIED;
                identityVerification.BlxVerifiedAt = DateTime.Now;
                user.IsVerified = true;
            }
            else if (stepStatus == EnumVerifyStepStatus.FAIL)
            {
                identityVerification.Status = EnumVerificationStatus.REJECTED;
                identityVerification.LastRejectReason = string.Join(" | ", reasons);
            }
            _unitOfWork.SaveChanges();
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }

        private static (double score, List<string> reasons) ScoreBlx(IdentityVerification identityVerification, BlxExtractResult extracted)
        {
            double score = 0;
            var reasons = new List<string>();

            if (string.IsNullOrWhiteSpace(extracted.LicenseNumber))
            {
                reasons.Add("Không đọc được số GPLX.");
                return (0, reasons);
            }
            if (!IsCarLicenseClass(extracted.LicenseClass))
            {
                reasons.Add("Hạng bằng không phải bằng lái ô tô.");
                return (0, reasons);
            }
            if (extracted.ExpiryDate > DateTime.Now)
            {
                reasons.Add("Bằng lái đã hết hạn.");
                return (0, reasons);
            }
            if (NormalizeName(extracted.FullName) == NormalizeName(identityVerification.CccdFullName))
            {
                score += W_FULLNAME;
            }
            else
            {
                reasons.Add("Họ tên trên BLX không trùng CCCD.");
            }
            if (extracted.DateOfBirth.HasValue && identityVerification.CccdDateOfBirth.HasValue && extracted.DateOfBirth.Value.Date == identityVerification.CccdDateOfBirth.Value.Date)
            {
                score += W_DOB;
            }
            else
            {
                reasons.Add("Ngày sinh trên BLX không trùng CCCD.");
            }
            if (!string.IsNullOrWhiteSpace(extracted.Nationality) && !string.IsNullOrWhiteSpace(identityVerification.CccdNationality))
            {
                if (NormalizeText(extracted.Nationality) == NormalizeText(identityVerification.CccdNationality))
                {
                    score += W_NATIONALITY;
                }
                else
                {
                    reasons.Add("Quốc tịch trên BLX không trùng CCCD.");
                }
            }
            if (!string.IsNullOrWhiteSpace(extracted.ResidentialAddress) && !string.IsNullOrWhiteSpace(identityVerification.CccdPlaceOfResidence))
            {
                if (IsAddressLikelyMatch(extracted.ResidentialAddress, identityVerification.CccdPlaceOfResidence))
                {
                    score += W_PERM_ADDR;
                }
                else
                {
                    reasons.Add("Địa chỉ thường trú trên BLX không trùng (hoặc không đủ giống) CCCD.");
                }
            }
            return (score, reasons);
        }

        private static bool IsCarLicenseClass(string? licenseClass)
        {
            if (string.IsNullOrWhiteSpace(licenseClass)) return false;

            var s = NormalizeText(licenseClass);
            s = s.Replace("HANG", "").Replace("HẠNG", "").Replace("HANG ", "").Trim();

            var compact = new string(s.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

            if (compact.StartsWith("A")) return false;

            return compact.StartsWith("B1")
                || compact.StartsWith("B2")
                || compact.StartsWith("C")
                || compact.StartsWith("D")
                || compact.StartsWith("E")
                || compact.StartsWith("F");
        }

        private static string NormalizeName(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = RemoveDiacritics(s).ToUpperInvariant();
            s = string.Join(' ', s.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return s;
        }

        private static string NormalizeText(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = RemoveDiacritics(s).ToUpperInvariant();
            s = s.Replace("\n", " ").Replace("\r", " ").Trim();
            s = string.Join(' ', s.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return s;
        }

        private static bool IsAddressLikelyMatch(string blxAddr, string cccdAddr)
        {
            var a = NormalizeAddress(blxAddr);
            var b = NormalizeAddress(cccdAddr);

            if (a.Length == 0 || b.Length == 0) return false;
            if (a.Contains(b) || b.Contains(a)) return true;

            var ta = a.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            var tb = b.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

            if (ta.Count == 0 || tb.Count == 0) return false;

            var common = ta.Intersect(tb).Count();
            var ratioA = (double)common / ta.Count;
            var ratioB = (double)common / tb.Count;

            return ratioA >= 0.6 || ratioB >= 0.6;
        }

        private static string NormalizeAddress(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";

            s = RemoveDiacritics(s).ToUpperInvariant();
            s = s.Replace("\n", " ").Replace("\r", " ");
            s = s.Replace(",", " ").Replace(".", " ").Replace("-", " ").Replace("/", " ");
            var noise = new HashSet<string> { "VIET", "NAM", "TP", "THANH", "PHO", "QUAN", "HUYEN", "XA", "PHUONG" };

            var tokens = s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Where(t => !noise.Contains(t))
                          .ToArray();

            return string.Join(' ', tokens);
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            var normalized = text.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                                  .ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
