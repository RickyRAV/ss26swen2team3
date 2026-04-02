using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Api.DTOs;
using TourPlanner.BL.Services;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/v1/map")]
[Authorize]
public class MapController : ControllerBase
{
    private static readonly Dictionary<string, string> ProfileMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Car"]      = "driving-car",
        ["Vacation"] = "driving-car",
        ["Bicycle"]  = "cycling-regular",
        ["Hiking"]   = "foot-hiking",
        ["Running"]  = "foot-walking",
    };

    private readonly IOrsService _ors;

    public MapController(IOrsService ors)
    {
        _ors = ors;
    }

    /// <summary>GET /api/v1/map/route?from=Vienna&amp;to=Salzburg&amp;transportType=Car</summary>
    [HttpGet("route")]
    public async Task<IActionResult> GetRoute(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] string transportType = "Car",
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            return BadRequest("Both 'from' and 'to' are required.");

        var profile = ProfileMap.GetValueOrDefault(transportType, "driving-car");
        var result = await _ors.GetRouteAsync(from, to, profile, ct);

        return Ok(new RouteResponse(result.Coordinates, result.DistanceMeters, result.DurationSeconds));
    }
}
