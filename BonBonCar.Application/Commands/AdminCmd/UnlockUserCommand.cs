using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BonBonCar.Application.Commands.AdminCmd
{
    public class UnlockUserCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
    }
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, MethodResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UnlockUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<bool>> Handle(UnlockUserCommand request, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                methodResult.AddErrorBadRequest("UserNotFound");
                return methodResult;
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}