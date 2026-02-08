using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.AuthCmdModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class ForgotPasswordCmd : ForgotPasswordCmdModel, IRequest<MethodResult<bool>>
    {
    }

    public class ForgotPasswordCmdHandler : IRequestHandler<ForgotPasswordCmd, MethodResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplate _emailTemplate;

        public ForgotPasswordCmdHandler(UserManager<User> userManager, IOtpService otpService, IEmailService emailService, IEmailTemplate emailTemplate)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
        }

        public async Task<MethodResult<bool>> Handle(ForgotPasswordCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Email));
                return methodResult;
            }
            // Generate OTP and send email
            var otp = Random.Shared.Next(100000, 999999).ToString();
            _otpService.SetOtp(user.Email ?? string.Empty, otp);
            var body = await _emailTemplate.GetOtpEmailBodyAsync(
                otp: otp,
                title: "Xác thực đổi mật khẩu tài khoản BonBonCar",
                previewLine: "Mã OTP để đổi mật khẩu tài khoản BonBonCar",
                messageLine: "Vui lòng nhập mã OTP bên dưới để hoàn tất quá trình đổi mật khẩu. Mã có hiệu lực trong 5 phút.",
                userName: request.Email,
                cancellationToken: cancellationToken);
            _ = Task.Run(() => _emailService.SendEmailAsync(request.Email, "BonBonCar Change Password OTP", body));
            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
