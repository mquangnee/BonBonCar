using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels;
using BonBonCar.Domain.Models.EntityModels;
using BonBonCar.Infrastructure.Identity;
using BonBonCar.Infrastructure.Services.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class RegisterCmd : RegisterCmdModel, IRequest<MethodResult<RegisterStartResultModel>>
    {
    }

    public class RegisterCmdHandler : IRequestHandler<RegisterCmd, MethodResult<RegisterStartResultModel>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;

        public RegisterCmdHandler(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHasher, IUnitOfWork unitOfWork, IEmailService emailService, IEmailTemplate emailTemplate, IPasswordValidator<ApplicationUser> passwordValidator)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
            _passwordValidator = passwordValidator;
        }

        public async Task<MethodResult<RegisterStartResultModel>> Handle(RegisterCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RegisterStartResultModel>();
            // Check if email already exists    
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Email));
                return methodResult;
            }
            // Create OTP
            var otp = RandomNumberGenerator.GetInt32(3, 1_000_000).ToString("D6");
            var otpHash = TokenUtil.Sha256Token(otp);
            // Hash password
            user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };
            var passwordResult = await _passwordValidator.ValidateAsync(_userManager, user, request.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.InValidFormat), nameof(request.Password), error.Description);
                }
                return methodResult;
            }
            var passwordHash = _passwordHasher.HashPassword(user, request.Password);
            // Create register session
            var session = new RegisterOtpSession
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                OtpHash = otpHash,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                FailedCount = 0,
                IsUsed = false,
                CreatedAt = DateTime.Now,
                LastSentAt = DateTime.Now
            };
            await _unitOfWork.RegisterOtpSessions.AddAsync(session);
            _unitOfWork.SaveChanges();
            // Send OTP email
            var body = await _emailTemplate.GetOtpEmailBodyAsync(
                otp: otp,
                title: "Xác thực đăng ký tài khoản BonBonCar",
                previewLine: "Mã OTP để hoàn tất đăng ký tài khoản BonBonCar",
                messageLine: "Vui lòng nhập mã OTP bên dưới để hoàn tất quá trình đăng ký. Mã có hiệu lực trong 5 phút.",
                userName: request.Email,
                cancellationToken: cancellationToken);
            _ = Task.Run(() => _emailService.SendEmailAsync(request.Email, "BonBonCar Registration OTP", body));
            var registeStartResultModel = new RegisterStartResultModel
            {
                RegisterSessionID = session.Id,
                ExpiresInSeconds = 5 * 60, // Hiển thị thời gian hết hạn
                MaskedEmail = MaskEmail(request.Email) // Hiển thị email đã được che mờ
            };
            methodResult.Result = registeStartResultModel;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
        private static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return "***" + email[at..];
            return email[0] + "***" + email[at..];
        }
    }
}
