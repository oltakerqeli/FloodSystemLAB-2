using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Repositories.Weather.Implementations
{
    public class AlertRepository : IAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public AlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Alert?> GetByIdAsync(int id)
        {
            return await _context.Alerts.Include(a => a.Location).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Alert>> GetAllAsync()
        {
            return await _context.Alerts.Include(a => a.Location).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Alert>> GetByLocationIdAsync(int locationId)
        {
            return await _context.Alerts
                .Where(a => a.LocationId == locationId)
                .Include(a => a.Location)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Alert> AddAsync(Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
            return alert;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}