using BonBonCar.Application.Commands.AuthCmd;
using BonBonCar.Application.Common;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BonBonCar.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// Route: /api/auth/register
        /// Method: POST
        /// </summary>
        /// <param name="command">
        /// Dữ liệu đăng ký người dùng: email, mật khẩu, xác nhận mật khẩu
        /// </param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(MethodResult<RegisterStartResultModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] RegisterCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Xác thực OTP cho đăng ký người dùng mới
        /// Route: /api/auth/register/verify-otp
        /// Method: POST
        /// </summary>
        /// <param name="command"><
        /// Dữ liệu xác thực OTP: email, mã OTP
        /// /param>
        [HttpPost("register/verify-otp")]
        [ProducesResponseType(typeof(MethodResult<AuthModel>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] VerifyRegisterOtpCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// Route: /api/auth/login
        /// Method: POST
        /// </summary>
        /// <param name="command">
        /// Dữ liệu đăng nhập người dùng: email, mật khẩu
        /// </param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(MethodResult<AuthModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
