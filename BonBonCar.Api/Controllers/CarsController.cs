using BonBonCar.Application.Commands.CarCmd;
using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.BasePriceQuery;
using BonBonCar.Application.Queries.BrandQuery;
using BonBonCar.Application.Queries.CarQueries;
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
        [ProducesResponseType(typeof(MethodResult<IList<Car>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCarsAvailable()
        {
            var query = new GetAvailableCarsQuery { };
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Lấy dữ liệu các hãng xe
        /// Route: /api/cars/get-brands
        /// Method: GET
        /// </summary>
        [HttpGet("get-brands")]
        [ProducesResponseType(typeof(MethodResult<IList<Brand>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllBrands()
        {
            var query = new GetAllBrandQuery { };
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Lấy dữ liệu các model xe theo hãng xe
        /// Route: /api/cars/get-models/{brandId}
        /// Method: GET
        /// </summary>
        [HttpGet("get-models/{brandId}")]
        [ProducesResponseType(typeof(MethodResult<IList<Model>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetModelsByBrandId([FromRoute] Guid brandId)
        {
            var commandResult = await _mediator.Send(new GetModelsByBrandIdQuery { BrandId = brandId}).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Lấy dữ liệu giá gốc theo loại xe
        /// Route: /api/cars/get-baseprices/{carType}
        /// Method: GET
        /// </summary>
        [HttpGet("get-baseprices/{carType}")]
        [ProducesResponseType(typeof(MethodResult<IList<BasePriceModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBasePrice([FromRoute] EnumCarType carType)
        {
            var commandResult = await _mediator.Send(new GetBasePriceQuery { CarType = carType }).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Upload xe cho thuê
        /// Route: /api/cars/upload-rental-car
        /// Method: POST
        /// </summary>
        [HttpPost("upload-rental-car")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UploadRentalCar([FromForm] CreateCarCmd command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Lấy danh sách xe cho thuê
        /// Route: /api/cars/get-rental-cars
        /// Method: GET
        /// </summary>
        [HttpGet("get-rental-cars")]
        [ProducesResponseType(typeof(MethodResult<IList<RentalCarModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRentalCars([FromQuery] GetRentalCarsQuery query)
        {
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Chỉnh sửa trạng thái xe
        /// Route: /api/cars/change-status
        /// Method: PUT
        /// </summary>
        [HttpPut("change-status/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangeStatusCar([FromRoute] Guid carId)
        {
            var command = new ChangeCarStatusCommand { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Gỡ bỏ xe cho thuê
        /// Route: /api/cars/remove
        /// Method: DELETE
        /// </summary>
        [HttpDelete("remove/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemoveRentalCar([FromRoute] Guid carId)
        {
            var command = new RemoveRentalCarCommand { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Lấy thông tin chi tiết xe
        /// Route: /api/cars/get-details
        /// Method: POST
        /// </summary>
        [HttpGet("details/{carId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCarDetail([FromRoute] Guid carId)
        {
            var command = new GetRentalCarDetailQuery { CarId = carId };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Cập nhật thông tin xe
        /// Route: /api/cars/update
        /// Method: PUT
        /// </summary>
        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateRentalCar([FromForm] UpdateRentalCarCommand command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        /// <summary>
        /// Search available cars by location and pickup/return datetime.
        /// Route: /api/cars/search
        /// Method: GET
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(MethodResult<IList<RentalCarModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchCars([FromQuery] SearchCarsQuery query)
        {
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
