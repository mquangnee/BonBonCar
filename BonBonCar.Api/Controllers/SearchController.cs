using BonBonCar.Application.Queries.CarQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BonBonCar.Api.Controllers
{
    [Route("api/cars")]
    [ApiController]
    [AllowAnonymous]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? location,
            [FromQuery(Name = "pickupDateTime")] string? pickupDateTimeRaw,
            [FromQuery(Name = "returnDateTime")] string? returnDateTimeRaw,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            // ✅ parse ISO 8601 an toàn, không phụ thuộc Culture
            DateTime? pickup = TryParseIsoLocal(pickupDateTimeRaw);
            DateTime? ret = TryParseIsoLocal(returnDateTimeRaw);

            var query = new SearchCarsQuery
            {
                Location = location,
                PickupDateTime = pickup,
                ReturnDateTime = ret,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, ct);
            return result.GetActionResult();
        }

        private static DateTime? TryParseIsoLocal(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            // Chấp nhận: 2026-02-24T18:00:00 hoặc 2026-02-24 18:00:00
            // Assumelocal: không đổi UTC
            if (DateTime.TryParse(
                    raw.Trim(),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out var dt))
            {
                return dt;
            }

            // fallback: parse exact ISO
            var formats = new[]
            {
                "yyyy-MM-dd'T'HH:mm:ss",
                "yyyy-MM-dd'T'HH:mm",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm"
            };

            if (DateTime.TryParseExact(
                    raw.Trim(),
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out dt))
            {
                return dt;
            }

            return null;
        }
    }
}
