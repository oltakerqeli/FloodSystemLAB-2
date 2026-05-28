using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Repositories.Weather.Implementations
{
    public class WeatherDataRepository : IWeatherDataRepository
    {
        private readonly ApplicationDbContext _context;

        public WeatherDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WeatherData?> GetByIdAsync(int id)
        {
            return await _context.WeatherData.Include(w => w.Location).FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<WeatherData>> GetByLocationIdAsync(int locationId)
        {
            return await _context.WeatherData
                .Where(w => w.LocationId == locationId)
                .OrderByDescending(w => w.RecordedAt)
                .ToListAsync();
        }

        public async Task<WeatherData?> GetLatestByLocationIdAsync(int locationId)
        {
            return await _context.WeatherData
                .Where(w => w.LocationId == locationId)
                .OrderByDescending(w => w.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherData> AddAsync(WeatherData weatherData)
        {
            await _context.WeatherData.AddAsync(weatherData);
            return weatherData;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}