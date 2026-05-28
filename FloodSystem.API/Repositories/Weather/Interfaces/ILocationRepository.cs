using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Repositories.Weather.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int id);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByNameAsync(string name);
        Task<IEnumerable<Location>> GetActiveLocationsAsync();
        Task<Location> AddAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}