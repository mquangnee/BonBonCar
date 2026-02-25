using AutoMapper;
using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BonBonCar.Application.Queries.AuthQuery
{
    public class GetUserProfileQuery : IRequest<MethodResult<UserModel>>
    {
    }

    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, MethodResult<UserModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<MethodResult<UserModel>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserModel>();

            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }

            var user = await _userManager.FindByIdAsync(userIdStr);
            if (user == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
                return methodResult;
            }
            methodResult.Result = _mapper.Map<UserModel>(user);
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
