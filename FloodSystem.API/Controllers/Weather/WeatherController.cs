using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.Services.Weather;

namespace FloodSystem.API.Controllers.Weather
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{locationId}/latest")]
        public async Task<IActionResult> GetLatestWeather(int locationId)
        {
            var weather = await _weatherService.GetLatestWeatherAsync(locationId);
            if (weather == null)
                return NotFound(new { message = "No weather data found" });

            return Ok(weather);
        }

        [HttpGet("{locationId}/history")]
        public async Task<IActionResult> GetWeatherHistory(int locationId)
        {
            var history = await _weatherService.GetWeatherHistoryAsync(locationId);
            return Ok(history);
        }

        [HttpGet("{locationId}/risk")]
        public async Task<IActionResult> GetRiskLevel(int locationId)
        {
            var risk = await _weatherService.GetRiskLevelForLocationAsync(locationId);
            return Ok(new { locationId, riskLevel = risk });
        }

        [HttpPost("fetch/{locationId}")]
        [Authorize(Roles = "Admin, Authority")]
        public async Task<IActionResult> FetchWeatherForLocation(int locationId)
        {
            try
            {
                var weather = await _weatherService.FetchAndProcessWeatherAsync(locationId);
                return Ok(new { message = "Weather fetched successfully", data = weather });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("fetch-all")]
        [Authorize(Roles = "Admin, Authority")]
        public async Task<IActionResult> FetchWeatherForAllLocations()
        {
            await _weatherService.FetchAndProcessAllLocationsAsync();
            return Ok(new { message = "Weather fetching initiated for all locations" });
        }
    }
}