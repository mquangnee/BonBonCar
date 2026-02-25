using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.AuthCmdModels;
using BonBonCar.Domain.Models.EntityModels;
using BonBonCar.Domain.Models.ServiceModel.TokenService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class VerifyRegisterOtpCmd : VerifyRegisterOtpCmdModel, IRequest<MethodResult<AuthModel>>
    {
    }

    public class VerifyRegisterOtpCmdHandler : IRequestHandler<VerifyRegisterOtpCmd, MethodResult<AuthModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public VerifyRegisterOtpCmdHandler(UserManager<User> userManager, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<MethodResult<AuthModel>> Handle(VerifyRegisterOtpCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<AuthModel>();
            var session = await _unitOfWork.RegisterOtpSessions.GetByIdAsync(Guid.Parse(request.SessionId));
            if (session == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.RegisterSessionNotExist), nameof(request.SessionId));
                return methodResult;
            }
            if (session.IsUsed)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.RegisterSessionUsed), nameof(request.SessionId));
                return methodResult;
            }
            if (session.ExpiredAt < DateTime.Now)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.OtpExpired), nameof(request.SessionId));
                return methodResult;
            }
            if (session.FailedCount >= 5)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.EnterOtpTooMuch), nameof(request.SessionId));
                return methodResult;
            }
            var inputOtpHash = TokenUtil.Sha256Token(request.Otp);
            if (inputOtpHash != session.OtpHash)
            {
                session.FailedCount += 1;
                _unitOfWork.RegisterOtpSessions.Update(session);
                _unitOfWork.SaveChanges();
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.OtpNotValid), nameof(request.Otp));
                return methodResult;
            }
            
            session.IsUsed = true;
            _unitOfWork.RegisterOtpSessions.Update(session);
            _unitOfWork.SaveChanges();

            var user = await _userManager.FindByEmailAsync(session.Email);
            if (user != null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(session.Email));
                return methodResult;
            }
            user = new User
            {
                UserName = session.Email,
                FullName = session.Username,
                Email = session.Email,
                PasswordHash = session.PasswordHash,
                IsVerified = false,
                CreatedAt = DateTime.Now
            };
            var createUserResult = await _userManager.CreateAsync(user);
            if (!createUserResult.Succeeded)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.RegistrationFailed));
                return methodResult;
            }
            await _userManager.AddToRoleAsync(user, "User");
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var accessToken = _jwtTokenService.CreateAccessToken(user.Id, user.FullName, user.Email, role);
            var refreshToken = _refreshTokenService.IssueAsync(user.Id);
            var authModel = new AuthModel
            {
                AccessToken = accessToken,
                RefreshToken = await refreshToken,
                Role = role
            };
            methodResult.Result = authModel;
            methodResult.StatusCode = StatusCodes.Status201Created;
            return methodResult;
        }
    }
}
