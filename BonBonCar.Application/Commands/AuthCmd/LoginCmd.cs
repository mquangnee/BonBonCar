using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IService;
using BonBonCar.Domain.Models.CmdModels.AuthCmdModels;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class LoginCmd : LoginCmdModel, IRequest<MethodResult<AuthModel>>   
    {
    }

    public class LoginCmdHandler : IRequestHandler<LoginCmd, MethodResult<AuthModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginCmdHandler(UserManager<User> userManager, SignInManager<User> signInManager, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<MethodResult<AuthModel>> Handle(LoginCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<AuthModel>();
            var user = await _userManager.FindByEmailAsync(request.Email);
            // Check email and password
            if (user == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.InvalidCredentials), nameof(request.Email));
                return methodResult;
            }
            var res = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!res.Succeeded)
            {
                methodResult.AddErrorBadRequest(nameof(EnumAuthErrorCode.InvalidCredentials), nameof(request.Password));
                return methodResult;
            }
            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;
            var accessToken = _jwtTokenService.CreateAccessToken(user.Id, user.FullName, request.Email, role);
            var refreshToken = await _refreshTokenService.IssueAsync(user.Id);
            var authModel = new AuthModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            methodResult.Result = authModel;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
