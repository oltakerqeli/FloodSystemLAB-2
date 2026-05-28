using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Repositories.Weather.Interfaces
{
    public interface ITrafficUpdateRepository
    {
        Task<TrafficUpdate?> GetByIdAsync(int id);
        Task<TrafficUpdate?> GetLatestByLocationIdAsync(int locationId);
        Task<IEnumerable<TrafficUpdate>> GetByLocationIdAsync(int locationId);
        Task<TrafficUpdate> AddAsync(TrafficUpdate trafficUpdate);
        Task SaveChangesAsync();
    }
}