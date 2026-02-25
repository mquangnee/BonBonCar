using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class ResetPasswordCmd : ForgotPasswordCmd, IRequest<MethodResult<bool>>
    {
    }

    public class ResetPasswordCmdHandler : IRequestHandler<ResetPasswordCmd, MethodResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplate _emailTemplate;

        public ResetPasswordCmdHandler(UserManager<User> userManager, IOtpService otpService, IEmailService emailService, IEmailTemplate emailTemplate)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
        }

        public async Task<MethodResult<bool>> Handle(ResetPasswordCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var user = await _userManager.FindByEmailAsync(request.Email ?? string.Empty);
            if (user == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Email));
                return methodResult;
            }
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized), nameof(request.ConfirmNewPassword));
                return methodResult;
            }

            var cachedOtp = _otpService.GetOtp(user.Email ?? string.Empty);
            if (cachedOtp == null || cachedOtp != request.Otp)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized), nameof(request.Otp));
                return methodResult;
            }

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, request.NewPassword ?? string.Empty);
            if (result.Succeeded)
            {
                _otpService.RemoveOtp(request.Email ?? string.Empty);
                var body = await _emailTemplate.GetChangePasswordSuccessEmailBodyAsync
                (
                    userName: user.FullName ?? user.Email ?? string.Empty,
                    time: DateTime.Now.ToString("f"),
                    cancellationToken: cancellationToken
                );
                _ = Task.Run(() => _emailService.SendEmailAsync(user.Email ?? string.Empty, "BonBonCar Password Changed Successfully", body));
                methodResult.Result = true;
                methodResult.StatusCode = StatusCodes.Status200OK;
                return methodResult;
            }
            methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.ChangePasswordFailed));
            return methodResult;
        }
    }
}
