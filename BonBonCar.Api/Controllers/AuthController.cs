using BonBonCar.Application.Commands.AuthCmd;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.AuthQuery;
using BonBonCar.Domain.Models.CmdModels.AuthCmdModels;
using BonBonCar.Domain.Models.EntityModels;
using BonBonCar.Domain.Models.ServiceModel.GoogleService;
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

        // Lấy profile người dùng
        [HttpGet("profile")]
        [ProducesResponseType(typeof(MethodResult<UserModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProfile()
        {
            var command = new GetUserProfileQuery();
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Đăng ký
        [HttpPost("register")]
        [ProducesResponseType(typeof(MethodResult<RegisterStartResultModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] RegisterCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Xác thực OTP cho đăng ký người dùng mới
        [HttpPost("register/verify-otp")]
        [ProducesResponseType(typeof(MethodResult<AuthModel>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] VerifyRegisterOtpCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Đăng nhập
        [HttpPost("login")]
        [ProducesResponseType(typeof(MethodResult<AuthModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Đăng xuất
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
            var commandResult = await _mediator.Send(new LogoutCmd { UserID = Guid.Parse(userId) }).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Quên mật khẩu - Yêu cầu đặt lại mật khẩu
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Đặt lại mật khẩu
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Xác minh CCCD
        [HttpPost("verify-cccd")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [ProducesResponseType(typeof(MethodResult<VerifyStepResult<CccdExtractResult>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyCccd([FromForm] VerifyCccdCmdModel form)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await form.FrontImage.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            var command = new VerifyCccdCmd
            {
                DocumentNumber = form.DocumentNumber,
                FullName = form.FullName,
                DateOfBirth = form.DateOfBirth,
                Gender = form.Gender,
                Nationality = form.Nationality,
                PlaceOfOrigin = form.PlaceOfOrigin,
                PlaceOfResidence = form.PlaceOfResidence,
                FrontImageBytes = bytes,
                FrontImageMimeType = form.FrontImage.ContentType
            };

            var commandResult = await _mediator.Send(command);
            return commandResult.GetActionResult();
        }

        // Xác minh BLX
        [HttpPost("verify-blx")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [ProducesResponseType(typeof(MethodResult<VerifyStepResult<BlxExtractResult>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyBlx([FromForm] VerifyBlxCmdModel form, CancellationToken ct)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await form.FrontImage.CopyToAsync(ms, ct);
                bytes = ms.ToArray();
            }

            var command = new VerifyBlxCmd
            {
                FrontImageBytes = bytes,
                FrontImageMimeType = form.FrontImage.ContentType
            };

            var commandResult = await _mediator.Send(command, ct);
            return commandResult.GetActionResult();
        }
    }
}
