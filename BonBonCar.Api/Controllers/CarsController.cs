using BonBonCar.Application.Commands.CarCmd;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.BasePriceQuery;
using BonBonCar.Application.Queries.BrandQuery;
using BonBonCar.Application.Queries.CarQuery;
using BonBonCar.Application.Queries.ModelQuery;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BonBonCar.Api.Controllers
{
    [Route("api/cars")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CarsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Lấy danh sách xe sẵn sàng cho thuê
        [HttpGet()]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MethodResult<IList<Car>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCarsAvailable()
        {
            var query = new GetAvailableCarsQuery { };
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy dữ liệu các hãng xe
        [HttpGet("get-brands")]
        [ProducesResponseType(typeof(MethodResult<IList<Brand>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllBrands()
        {
            var query = new GetAllBrandQuery { };
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy dữ liệu các model xe theo hãng xe
        [HttpGet("get-models/{brandId}")]
        [ProducesResponseType(typeof(MethodResult<IList<Model>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetModelsByBrandId([FromRoute] Guid brandId)
        {
            var commandResult = await _mediator.Send(new GetModelsByBrandIdQuery { BrandId = brandId}).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy dữ liệu giá gốc theo loại xe
        [HttpGet("get-baseprices/{carType}")]
        [ProducesResponseType(typeof(MethodResult<IList<BasePriceModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBasePrice([FromRoute] EnumCarType carType)
        {
            var commandResult = await _mediator.Send(new GetBasePriceQuery { CarType = carType }).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Upload xe cho thuê
        [HttpPost("upload-rental-car")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UploadRentalCar([FromForm] CreateCarCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy danh sách xe cho thuê
        [HttpGet("get-rental-cars")]
        [ProducesResponseType(typeof(MethodResult<IList<RentalCarModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRentalCars([FromQuery] GetRentalCarsQuery query)
        {
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Chỉnh sửa trạng thái xe
        [HttpPut("change-status/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangeStatusCar([FromRoute] Guid carId)
        {
            var command = new ChangeCarStatusCommand { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Gỡ bỏ xe cho thuê
        [HttpDelete("remove/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemoveRentalCar([FromRoute] Guid carId)
        {
            var command = new RemoveRentalCarCommand { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy thông tin chi tiết xe
        [HttpGet("details/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCarDetail([FromRoute] Guid carId)
        {
            var command = new GetRentalCarDetailQuery { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Cập nhật thông tin xe
        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateRentalCar([FromForm] UpdateRentalCarCommand command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        // Lấy thông tin chi tiết xe muốn thuê
        [HttpGet("get-car-rent-detail")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MethodResult<CarForRentModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCarForRent([FromQuery] Guid carId, [FromQuery] DateTime? pickupDateTime, [FromQuery] DateTime? returnDateTime, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var pickupTime = pickupDateTime ?? now.AddHours(1);
            var returnTime = returnDateTime ?? pickupTime.AddHours(4);
            if (returnTime <= pickupTime)
            {
                return BadRequest(new   
                {
                    isOK = false,
                    errorMessages = new[] { "Thời gian trả xe phải sau thời gian nhận xe" }
                });
            }
            var query = new GetCarDetailForRentQuery
            {
                CarId = carId,
                PickupDateTime = pickupTime,
                ReturnDateTime = returnTime
            };
            var commandResult = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
