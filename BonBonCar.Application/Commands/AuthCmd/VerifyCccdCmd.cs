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
using System.Security.Claims;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class VerifyCccdCmd : VerifyCccdCmdModel, IRequest<MethodResult<VerifyStepResult<CccdExtractResult>>>
    {
        public byte[]? FrontImageBytes { get; set; }
        public string? FrontImageMimeType { get; set; }
    }

    public class VerifyCccdCmdHandler : IRequestHandler<VerifyCccdCmd, MethodResult<VerifyStepResult<CccdExtractResult>>>
    {
        private const double W_CCCD = 0.35;
        private const double W_FULLNAME = 0.15;
        private const double W_DOB = 0.15;
        private const double W_GENDER = 0.10;
        private const double W_NATIONALITY = 0.10;
        private const double W_ORIGIN = 0.075;
        private const double W_PERM_ADDR = 0.075;

        private readonly IDocumentAiService _documentAiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
 
        public VerifyCccdCmdHandler(IDocumentAiService documentAiService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _documentAiService = documentAiService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<VerifyStepResult<CccdExtractResult>>> Handle(VerifyCccdCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<VerifyStepResult<CccdExtractResult>>();

            if (request.DocumentNumber == null || request.FullName == null || request.DateOfBirth == null || request.Gender == null || request.Nationality == null || request.PlaceOfOrigin == null || request.PlaceOfResidence == null || request.FrontImageBytes == null || request.FrontImageMimeType == null)
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

            var extracted = await _documentAiService.ExtractCccdAsync(request.FrontImageBytes, request.FrontImageMimeType, cancellationToken);
            var (score, reasons) = ScoreCccd(request, extracted);
            var stepStatus = score >= 0.85 ? EnumVerifyStepStatus.PASS : EnumVerifyStepStatus.FAIL;

            var identityVerification = await _unitOfWork.IdentityVerification.GetByUserIdAsync(userId);
            if (identityVerification == null)
            {
                identityVerification = new IdentityVerification
                {
                    UserId = userId
                };
                await _unitOfWork.IdentityVerification.AddAsync(identityVerification);
                _unitOfWork.SaveChanges();
            }

            if (stepStatus == EnumVerifyStepStatus.PASS)
            {
                identityVerification.CccdFullName = extracted.FullName;
                identityVerification.CccdDateOfBirth = extracted.DateOfBirth;
                identityVerification.CccdNationality = extracted.Nationality;
                identityVerification.CccdPlaceOfResidence = extracted.PlaceOfResidence;
                identityVerification.Status = EnumVerificationStatus.CCCDPASSED;
                identityVerification.CccdVerifiedAt = DateTime.Now;
                identityVerification.LastRejectReason = null;
            }
            else if (stepStatus == EnumVerifyStepStatus.FAIL)
            {
                identityVerification.Status = EnumVerificationStatus.REJECTED;
                identityVerification.LastRejectReason = string.Join(" | ", reasons);
            }
            _unitOfWork.IdentityVerification.Update(identityVerification);
            _unitOfWork.SaveChanges();

            var result = new VerifyStepResult<CccdExtractResult>
            {
                Status = stepStatus,
                Score = Math.Round(score, 2),
                Reasons = reasons,
                Extracted = extracted
            };
            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }

        private static (double score, List<string> reasons) ScoreCccd(VerifyCccdCmd request, CccdExtractResult extracted)
        {
            var reasons = new List<string>();
            double score = 0;

            if (!extracted.ExpiryDate.HasValue)
            {
                reasons.Add("Không đọc được ngày hết hạn");
            }
            if (extracted.ExpiryDate.Value.Date < DateTime.Today)
            {
                reasons.Add($"CCCD đã hết hạn (hết hạn: {extracted.ExpiryDate:dd/MM/yyyy}).");
            }

            var inputNo = OnlyDigits(request.DocumentNumber ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(extracted.DocumentNumber) && extracted.DocumentNumber.Trim() == inputNo)
            {
                score += W_CCCD;
            }
            else
            {
                reasons.Add("Số CCCD không khớp hoặc không đọc được.");
            }

            if (!string.IsNullOrWhiteSpace(extracted.FullName) && NormalizeText(extracted.FullName) == NormalizeText(request.FullName))
            {
                score += W_FULLNAME;
            }
            else
            {
                reasons.Add("Họ tên không khớp hoặc không đọc được.");
            }

            if (extracted.DateOfBirth.HasValue && extracted.DateOfBirth.Value.Date == request.DateOfBirth.Date)
            {
                score += W_DOB;
            }
            else
            {
                reasons.Add("Ngày sinh không khớp hoặc không đọc được.");
            }

            if (!string.IsNullOrWhiteSpace(extracted.Gender) && NormalizeText(extracted.Gender) == NormalizeText(request.Gender))
            {
                score += W_GENDER;
            }
            else
            {
                reasons.Add("Giới tính không khớp hoặc không đọc được.");
            }

            if (!string.IsNullOrWhiteSpace(extracted.Nationality) && NormalizeText(extracted.Nationality) == NormalizeText(request.Nationality))
            {
                score += W_NATIONALITY;
            }
            else
            {
                reasons.Add("Quốc tịch không khớp hoặc không đọc được.");
            }

            if (!string.IsNullOrWhiteSpace(extracted.PlaceOfOrigin) && NormalizeText(extracted.PlaceOfOrigin) == NormalizeText(request.PlaceOfOrigin))
            {
                score += W_ORIGIN;
            }
            else
            {
                reasons.Add("Quê quán không khớp hoặc không đọc được.");
            }

            if (!string.IsNullOrWhiteSpace(extracted.PlaceOfResidence) && NormalizeText(extracted.PlaceOfResidence) == NormalizeText(request.PlaceOfResidence))
            {
                score += W_PERM_ADDR;
            }
            else
            {
                reasons.Add("Địa chỉ thường trú không khớp hoặc không đọc được.");
            }
            return (score, reasons);
        }

        private static string OnlyDigits(string s)
        {
            return new string((s ?? "").Where(char.IsDigit).ToArray());
        }

        private static string NormalizeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Replace("\r\n", ",")
                .Replace("\n", ",")
                .Replace("\r", ",")
                .Replace(",", " ")
                .Replace(".", " ")
                .ToLowerInvariant()
                .Trim()
                .Replace("  ", " ");
        }
    }
}
