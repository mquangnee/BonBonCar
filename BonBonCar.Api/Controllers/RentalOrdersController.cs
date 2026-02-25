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
            var s = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(s, out var id)) throw new Exception("Không xác định được người dùng!");
            return id;
        }

        [HttpPost("hold")]
        [Authorize]
        public async Task<ActionResult<CreateHoldResponse>> CreateHold([FromBody] CreateHoldRequest req, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var customerId))
                return Unauthorized();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            // VNPay sẽ redirect trình duyệt về BE endpoint này sau khi thanh toán
            var vnpayReturnUrl = $"{Request.Scheme}://{Request.Host}/api/vnpay/return";

            var result = await _mediator.Send(new CreateHoldCommand { CustomerId = customerId, ClientIp = ip, VnpayReturnUrl = vnpayReturnUrl, Request = req }, ct);
            return Ok(new { isOK = true, result });
        }

        [HttpGet("my/active")]
        [ProducesResponseType(typeof(MethodResult<MyActiveRentalsResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> MyActive(CancellationToken ct)
        {
            var userId = GetUserId();
            var q = new GetMyActiveRentalOrdersQuery { CustomerId = userId };
            var r = await _mediator.Send(q, ct);
            return r.GetActionResult();
        }

        [HttpPost("{rentalOrderId:guid}/cancel")]
        [ProducesResponseType(typeof(MethodResult<CancelRentalResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Cancel([FromRoute] Guid rentalOrderId, CancellationToken ct)
        {
            var userId = GetUserId();
            var cmd = new CancelRentalOrderCommand
            {
                CustomerId = userId,
                RentalOrderId = rentalOrderId,
                NowLocal = DateTime.Now
            };
            var r = await _mediator.Send(cmd, ct);
            return r.GetActionResult();
        }

        [HttpPost("{rentalOrderId:guid}/pay-rental-fee")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResult<CreateRentalFeePaymentResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PayRentalFee(
            [FromRoute] Guid rentalOrderId,
            [FromBody] CreateRentalFeePaymentRequest body,
            CancellationToken ct)
        {
            var userId = GetUserId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var vnpayReturnUrl = $"{Request.Scheme}://{Request.Host}/api/vnpay/return";

            var cmd = new CreateRentalFeePaymentCommand
            {
                CustomerId = userId,
                RentalOrderId = rentalOrderId,
                ClientIp = ip,
                VnpayReturnUrl = vnpayReturnUrl,
                Request = body ?? new CreateRentalFeePaymentRequest()
            };

            var r = await _mediator.Send(cmd, ct);
            return r.GetActionResult();
        }
    }
}
