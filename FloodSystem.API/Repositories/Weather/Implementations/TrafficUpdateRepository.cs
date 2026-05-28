using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Repositories.Weather.Implementations
{
    public class TrafficUpdateRepository : ITrafficUpdateRepository
    {
        private readonly ApplicationDbContext _context;

        public TrafficUpdateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrafficUpdate?> GetByIdAsync(int id)
        {
            return await _context.TrafficUpdates.Include(t => t.Location).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TrafficUpdate?> GetLatestByLocationIdAsync(int locationId)
        {
            return await _context.TrafficUpdates
                .Where(t => t.LocationId == locationId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TrafficUpdate>> GetByLocationIdAsync(int locationId)
        {
            return await _context.TrafficUpdates
                .Where(t => t.LocationId == locationId)
                .Include(t => t.Location)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TrafficUpdate> AddAsync(TrafficUpdate trafficUpdate)
        {
            await _context.TrafficUpdates.AddAsync(trafficUpdate);
            return trafficUpdate;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}