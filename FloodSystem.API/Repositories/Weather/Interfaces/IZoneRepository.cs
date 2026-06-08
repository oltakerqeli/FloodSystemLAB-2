using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Repositories.Weather.Interfaces
{
    public interface IZoneRepository
    {
        Task<Zone?> GetByIdAsync(int id);
        Task<IEnumerable<Zone>> GetAllAsync();
         Task<IEnumerable<Zone>> GetAllIncludingInactiveAsync(); 
        Task<Zone?> GetByNameAsync(string name);
        Task<Zone> AddAsync(Zone zone);
        Task<Zone> UpdateAsync(Zone zone);
        Task<bool> DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}