using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.DTOs.Search;
using FloodSystem.API.Services.Search;

namespace FloodSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("alerts")]
        public async Task<IActionResult> SearchAlerts([FromBody] AlertSearchDto dto)
            => Ok(await _searchService.SearchAlertsAsync(dto));

        [HttpPost("locations")]
        public async Task<IActionResult> SearchLocations([FromBody] LocationSearchDto dto)
            => Ok(await _searchService.SearchLocationsAsync(dto));

        [HttpPost("weather")]
        public async Task<IActionResult> SearchWeatherData([FromBody] WeatherDataSearchDto dto)
            => Ok(await _searchService.SearchWeatherDataAsync(dto));

        [HttpPost("traffic")]
        public async Task<IActionResult> SearchTrafficUpdates([FromBody] TrafficUpdateSearchDto dto)
            => Ok(await _searchService.SearchTrafficUpdatesAsync(dto));

        [HttpPost("zones")]
        public async Task<IActionResult> SearchZones([FromBody] ZoneSearchDto dto)
            => Ok(await _searchService.SearchZonesAsync(dto));
    }
}