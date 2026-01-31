using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IService;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Commands.AuthCmd
{
    public class LogoutCmd : IRequest<MethodResult<bool>>
    {
        public Guid UserID { get; set; }
    }

    public class LogoutCmdHandler : IRequestHandler<LogoutCmd, MethodResult<bool>>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public LogoutCmdHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<MethodResult<bool>> Handle(LogoutCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var refeshToken = await _refreshTokenService.GetByUserIdAsync(request.UserID);
            if (refeshToken == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }

            if (refeshToken.RevokedAt == null)
            {
                await _refreshTokenService.RevokeAsync(refeshToken.TokenHash);
            }

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
