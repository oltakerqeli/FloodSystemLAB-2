using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Repositories.Weather.Interfaces
{
    public interface IAlertRepository
    {
        Task<Alert?> GetByIdAsync(int id);
        Task<IEnumerable<Alert>> GetAllAsync();
        Task<IEnumerable<Alert>> GetByLocationIdAsync(int locationId);
        Task<Alert> AddAsync(Alert alert);
        Task<Alert?> GetLatestByLocationIdAsync(int locationId);
        Task SaveChangesAsync();
    }
}