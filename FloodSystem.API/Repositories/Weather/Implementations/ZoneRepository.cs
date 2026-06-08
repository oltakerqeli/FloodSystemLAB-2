using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Repositories.Weather.Implementations
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly ApplicationDbContext _context;

        public ZoneRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Zone?> GetByIdAsync(int id)
        {
            return await _context.Zones.FirstOrDefaultAsync(z => z.Id == id);
        }

    
        public async Task<IEnumerable<Zone>> GetAllAsync()
        {
            return await _context.Zones.Where(z => z.IsActive).ToListAsync();
        }

       
        public async Task<IEnumerable<Zone>> GetAllIncludingInactiveAsync()
        {
            return await _context.Zones.ToListAsync();
        }

        public async Task<Zone?> GetByNameAsync(string name)
        {
            return await _context.Zones.FirstOrDefaultAsync(z => z.Name == name);
        }

        public async Task<Zone> AddAsync(Zone zone)
        {
            await _context.Zones.AddAsync(zone);
            return zone;
        }

        public async Task<Zone> UpdateAsync(Zone zone)
        {
            zone.UpdatedAt = DateTime.UtcNow;
            _context.Zones.Update(zone);
            return zone;
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            var zone = await GetByIdAsync(id);
            if (zone == null) return false;
            
            // Hard delete - fshije plotësisht nga database
            _context.Zones.Remove(zone);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}