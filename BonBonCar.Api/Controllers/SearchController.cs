using BonBonCar.Application.Common;
using BonBonCar.Application.Queries.CarQueries;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BonBonCar.Api.Controllers
{
    /// <summary>
    /// Public search endpoints (no authentication required).
    /// </summary>
    [Route("api/search")]
    [ApiController]
    [AllowAnonymous]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search available cars by location and pickup/return datetime.
        /// New route: /api/search/cars
        /// Backward-compatible route: /api/cars/search
        /// Method: GET
        /// </summary>
        [HttpGet("cars")]
        [HttpGet("/api/cars/search")]
        [ProducesResponseType(typeof(MethodResult<IList<RentalCarModel>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchCars([FromQuery] SearchCarsQuery query)
        {
            var commandResult = await _mediator.Send(query).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}

