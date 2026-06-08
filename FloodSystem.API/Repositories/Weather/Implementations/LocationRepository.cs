using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Repositories.Weather.Implementations
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.Where(l => l.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetAllIncludingInactiveAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location?> GetByNameAsync(string name)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.Name == name);
        }

        public async Task<IEnumerable<Location>> GetActiveLocationsAsync()
        {
            return await _context.Locations.Where(l => l.IsActive).ToListAsync();
        }

        public async Task<Location> AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return location;
        }

        public async Task<bool> DeleteAsync(int id)
{
    var location = await GetByIdAsync(id);
    if (location == null) return false;
    
    // HARD DELETE - fshije plotësisht nga database (jo soft delete)
    _context.Locations.Remove(location);
    return true;
}

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Locations.AnyAsync(l => l.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}