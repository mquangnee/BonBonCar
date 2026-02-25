using BonBonCar.Application.Commands.AdminCmd;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.Admin;
using BonBonCar.Domain.Models.QueryModels.Admin;
using BonBonCar.Domain.Enums.Car;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BonBonCar.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Lấy danh sách người dùng
        [HttpGet("users")]
        [ProducesResponseType(typeof(MethodResult<ListUsersModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var command = new GetUsersQuery
            {
                Page = page,
                PageSize = pageSize
            };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Khóa tài khoản người dùng
        [HttpPost("users/{userId}/lock")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> LockUser([FromRoute] Guid userId)
        {
            var command = new LockUserCommand { UserId = userId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Mở khóa tài khoản người dùng
        [HttpPost("users/{userId}/unlock")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UnlockUser([FromRoute] Guid userId)
        {
            var command = new UnlockUserCommand { UserId = userId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy danh sách xe cho thuê
        [HttpGet("cars")]
        [ProducesResponseType(typeof(MethodResult<ListCarsModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCars([FromQuery] GetCarsQuery query, CancellationToken ct)
        {
            var commandResult = await _mediator.Send(query, ct).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Vô hiệu hóa xe
        [HttpPost("cars/{carId}/disable")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DisableCar([FromRoute] Guid carId, CancellationToken cancellationToken)
        {
            var command = new SetCarStatusCommand
            {
                CarId = carId,
                Status = EnumCarStatus.Disabled
            };
            var commandResult = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Bỏ vô hiệu hóa xe
        [HttpPost("cars/{carId}/enable")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EnableCar([FromRoute] Guid carId, CancellationToken cancellationToken)
        {
            var command = new SetCarStatusCommand
            {
                CarId = carId,
                Status = EnumCarStatus.Available
            };
            var commandResult = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Thống kê doanh thu
        [HttpGet("revenue/summary")]
        [ProducesResponseType(typeof(MethodResult<RevenueSummaryModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRevenueSummary([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
        {
            var command = new GetRevenueSummaryQuery
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var commandResult = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}

