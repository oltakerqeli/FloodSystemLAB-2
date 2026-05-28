using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.Services.Weather;

namespace FloodSystem.API.Controllers.Weather
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrafficController : ControllerBase
    {
        private readonly TrafficService _trafficService;

        public TrafficController(TrafficService trafficService)
        {
            _trafficService = trafficService;
        }

        [HttpGet("safe-routes")]
        public async Task<IActionResult> GetSafeRoutes()
        {
            var routes = await _trafficService.GetSafeRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("location/{locationId}/latest")]
        public async Task<IActionResult> GetLatestTraffic(int locationId)
        {
            var traffic = await _trafficService.GetLatestTrafficAsync(locationId);
            if (traffic == null)
                return NotFound(new { message = "No traffic data found" });

            return Ok(traffic);
        }

        [HttpGet("location/{locationId}/history")]
        public async Task<IActionResult> GetTrafficHistory(int locationId)
        {
            var history = await _trafficService.GetTrafficHistoryAsync(locationId);
            return Ok(history);
        }
    }
}