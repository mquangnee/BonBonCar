using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.BasePriceQuery;
using BonBonCar.Application.Queries.BrandQuery;
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
    }
}
