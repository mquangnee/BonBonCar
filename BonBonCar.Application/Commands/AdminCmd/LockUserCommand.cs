using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AdminCmd
{
    public class LockUserCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
    }
    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, MethodResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public LockUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<bool>> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                methodResult.AddErrorBadRequest("UserNotFound");
                return methodResult;
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

