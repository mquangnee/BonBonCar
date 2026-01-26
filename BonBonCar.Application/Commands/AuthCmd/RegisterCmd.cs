using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels;
using BonBonCar.Domain.Models.EntityModels;
using BonBonCar.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class RegisterCmd : RegisterCmdModel, IRequest<MethodResult<TokenModel>> 
    {
    }

    public class RegisterCmdHandler : IRequestHandler<RegisterCmd, MethodResult<TokenModel>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public RegisterCmdHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<MethodResult<TokenModel>> Handle(RegisterCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<TokenModel>();
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (request.Password != request.ConfirmPassword)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.InValidFormat), nameof(request.ConfirmPassword));
                return methodResult;
            }
            if (user != null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Email));
                return methodResult;
            }
            user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };
            var res = await _userManager.CreateAsync(user, request.Password);
            if (!res.Succeeded)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.RegistrationFailed));
                return methodResult;
            }
            await _userManager.AddToRoleAsync(user, "User");
            var role = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateAccessToken(user.Id, user.Email, role);
            var refreshToken = await _refreshTokenService.IssueAsync(user.Id);
            methodResult.Result = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            methodResult.StatusCode = StatusCodes.Status201Created;
            return methodResult;
        }
    }
}
