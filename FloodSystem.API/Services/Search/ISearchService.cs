using FloodSystem.API.DTOs.Search;
using FloodSystem.API.DTOs.Weather;

namespace FloodSystem.API.Services.Search
{
    public interface ISearchService
    {
        Task<SearchResultDto<AlertDto>> SearchAlertsAsync(AlertSearchDto dto);
        Task<SearchResultDto<LocationDto>> SearchLocationsAsync(LocationSearchDto dto);
        Task<SearchResultDto<WeatherDataDto>> SearchWeatherDataAsync(WeatherDataSearchDto dto);
        Task<SearchResultDto<TrafficUpdateDto>> SearchTrafficUpdatesAsync(TrafficUpdateSearchDto dto);
        Task<SearchResultDto<ZoneDto>> SearchZonesAsync(ZoneSearchDto dto);
    }
}