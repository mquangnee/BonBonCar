using BonBonCar.Application.Commands.AuthCmd;
using BonBonCar.Application.Common;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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
        /// <param name="command"> Dữ liệu đăng ký người dùng: email, mật khẩu, xác nhận mật khẩu
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
        /// <param name="command">< Dữ liệu xác thực OTP: email, mã OTP
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
        /// <param name="command"> Dữ liệu đăng nhập người dùng: email, mật khẩu
        /// </param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(MethodResult<AuthModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Đăng xuất người dùng
        /// Route: /api/auth/logout
        /// Method: POST
        /// </summary>
        /// <param name="command"></param> Dữ liệu đăng xuất người dùng: refresh token
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _mediator.Send(new LogoutCmd { UserID = Guid.Parse(userId) }).ConfigureAwait(false);
            return result.GetActionResult();
        }

        /// <summary>
        /// Quên mật khẩu - Yêu cầu đặt lại mật khẩu
        /// Route: /api/auth/forgot-password
        /// Method: POST
        /// </summary>
        /// <param name="command"></param> Dữ liệu yêu cầu đặt lại mật khẩu: email
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCmd command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result.GetActionResult();
        }

        /// <summary>
        /// Đặt lại mật khẩu
        /// Route: /api/auth/reset-password
        /// Method: POST
        /// </summary>
        /// <param name="command"></param> Dữ liệu đặt lại mật khẩu: email, mã OTP, mật khẩu mới, xác nhận mật khẩu mới
        /// <returns></returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCmd command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return result.GetActionResult();
        }
    }
}
