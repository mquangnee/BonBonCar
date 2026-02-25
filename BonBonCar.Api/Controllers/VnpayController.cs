using BonBonCar.Application.Commands.VnpayCmd;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.PaymentQuery;
using BonBonCar.Domain.Models.ServiceModel.VnpayService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace BonBonCar.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/vnpay")]
    public class VnpayController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly VnpayOptions _options;

        public VnpayController(IMediator mediator, IOptions<VnpayOptions> options)
        {
            _mediator = mediator;
            _options = options.Value;
        }

        [HttpGet("ipn")]
        [ProducesResponseType(typeof(MethodResult<VnpayIpnResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Ipn(CancellationToken ct)
        {
            var res = await _mediator.Send(new ProcessVnpayIpnCommand(Request.Query), ct);
            return res.GetActionResult();
        }

        [HttpGet("return")]
        public async Task<IActionResult> Return(CancellationToken ct)
        {
            var commandResult = await _mediator.Send(new ProcessVnpayIpnCommand(Request.Query), ct);

            var clientUrl = string.IsNullOrWhiteSpace(_options.ReturnUrl) ? "/" : _options.ReturnUrl;

            var txnRef = Request.Query["vnp_TxnRef"].FirstOrDefault();
            var responseCode = Request.Query["vnp_ResponseCode"].FirstOrDefault();
            var rspCode = commandResult.Result?.RspCode;

            var join = clientUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
            var redirect = $"{clientUrl}{join}txnRef={Uri.EscapeDataString(txnRef ?? string.Empty)}&vnp_ResponseCode={Uri.EscapeDataString(responseCode ?? string.Empty)}&vnp_RspCode={Uri.EscapeDataString(rspCode ?? string.Empty)}";
            return Redirect(redirect);
        }

        [HttpGet("status/{txnRef}")]
        public async Task<IActionResult> Status(string txnRef, CancellationToken ct)
        {
            var res = await _mediator.Send(new GetPaymentStatusQuery(txnRef), ct);
            if (res == null) return NotFound();
            return Ok(res);
        }
    }
}
