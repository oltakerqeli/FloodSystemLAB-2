using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Repositories.Weather.Interfaces
{
    public interface IWeatherDataRepository
    {
        Task<WeatherData?> GetByIdAsync(int id);
        Task<IEnumerable<WeatherData>> GetByLocationIdAsync(int locationId);
        Task<WeatherData?> GetLatestByLocationIdAsync(int locationId);
        Task<WeatherData> AddAsync(WeatherData weatherData);
        Task SaveChangesAsync();
    }
}