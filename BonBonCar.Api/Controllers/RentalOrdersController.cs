using BonBonCar.Application.Commands.RentalOrderCmd;
using BonBonCar.Application.Commands.Rentals;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.Rentals;
using BonBonCar.Domain.Models.CmdModels.RentalOrderCmdModels;
using BonBonCar.Domain.Models.QueryModels.RentalOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace BonBonCar.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/rental-orders")]
    public sealed class RentalOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _http;

        public RentalOrdersController(IMediator mediator, IHttpContextAccessor http)
        {
            _mediator = mediator;
            _http = http;
        }

        private Guid GetUserId()
        {
            var userIdStr = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) throw new Exception("Không xác định được người dùng!");
            return userId;
        }

        [HttpPost("hold")]
        [Authorize]
        public async Task<ActionResult<CreateHoldResponse>> CreateHold([FromBody] CreateHoldRequest req, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var customerId))
                return Unauthorized();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var vnpayReturnUrl = $"{Request.Scheme}://{Request.Host}/api/vnpay/return";
            var result = await _mediator.Send(new CreateHoldCommand { CustomerId = customerId, ClientIp = ip, VnpayReturnUrl = vnpayReturnUrl, Request = req }, ct);
            return Ok(new { isOK = true, result });
        }

        [HttpGet("my/active")]
        [ProducesResponseType(typeof(MethodResult<MyActiveRentalsResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MyActive(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var query = new GetMyActiveRentalOrdersQuery { CustomerId = userId };
            var queryResult = await _mediator.Send(query, cancellationToken);
            return queryResult.GetActionResult();
        }

        [HttpPost("{rentalOrderId:guid}/cancel")]
        [ProducesResponseType(typeof(MethodResult<CancelRentalResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Cancel([FromRoute] Guid rentalOrderId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var command = new CancelRentalOrderCommand
            {
                CustomerId = userId,
                RentalOrderId = rentalOrderId,
                TimeNow = DateTime.Now
            };
            var commandResult = await _mediator.Send(command, cancellationToken);
            return commandResult.GetActionResult();
        }

        [HttpPost("{rentalOrderId:guid}/pay-rental-fee")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResult<CreateRentalFeePaymentResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PayRentalFee(
            [FromRoute] Guid rentalOrderId,
            [FromBody] CreateRentalFeePaymentRequest body,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var vnpayReturnUrl = $"{Request.Scheme}://{Request.Host}/api/vnpay/return";
            var command = new CreateRentalFeePaymentCommand
            {
                CustomerId = userId,
                RentalOrderId = rentalOrderId,
                ClientIp = ip,
                VnpayReturnUrl = vnpayReturnUrl,
                Request = body ?? new CreateRentalFeePaymentRequest()
            };
            var commandResult = await _mediator.Send(command, cancellationToken);
            return commandResult.GetActionResult();
        }
    }
}
