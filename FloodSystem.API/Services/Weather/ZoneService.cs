using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Services.Weather
{
    public class ZoneService
    {
        private readonly IZoneRepository _zoneRepo;
        private readonly ILocationRepository _locationRepo;

        public ZoneService(IZoneRepository zoneRepo, ILocationRepository locationRepo)
        {
            _zoneRepo = zoneRepo;
            _locationRepo = locationRepo;
        }

        public async Task<IEnumerable<ZoneDto>> GetAllZonesAsync()
        {
            var zones = await _zoneRepo.GetAllAsync();
            return zones.Select(z => new ZoneDto
            {
                Id = z.Id,
                Name = z.Name,
                Description = z.Description,
                CriticalRainfallThreshold = z.CriticalRainfallThreshold,
                IsActive = z.IsActive,
                CreatedAt = z.CreatedAt
            });
        }

        public async Task<ZoneDto?> GetZoneByIdAsync(int id)
        {
            var zone = await _zoneRepo.GetByIdAsync(id);
            if (zone == null) return null;

            return new ZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                CriticalRainfallThreshold = zone.CriticalRainfallThreshold,
                IsActive = zone.IsActive,
                CreatedAt = zone.CreatedAt
            };
        }

        public async Task<ZoneDto> CreateZoneAsync(CreateZoneDto dto)
        {
            var existing = await _zoneRepo.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new Exception($"Zone '{dto.Name}' already exists");

            var zone = new Zone
            {
                Name = dto.Name,
                Description = dto.Description,
                CriticalRainfallThreshold = dto.CriticalRainfallThreshold,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _zoneRepo.AddAsync(zone);
            await _zoneRepo.SaveChangesAsync();

            return new ZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                CriticalRainfallThreshold = zone.CriticalRainfallThreshold,
                IsActive = zone.IsActive,
                CreatedAt = zone.CreatedAt
            };
        }

        public async Task<ZoneDto?> UpdateZoneAsync(int id, CreateZoneDto dto)
        {
            var zone = await _zoneRepo.GetByIdAsync(id);
            if (zone == null) return null;

            zone.Name = dto.Name;
            zone.Description = dto.Description;
            zone.CriticalRainfallThreshold = dto.CriticalRainfallThreshold;
            zone.UpdatedAt = DateTime.UtcNow;

            await _zoneRepo.UpdateAsync(zone);
            await _zoneRepo.SaveChangesAsync();

            return new ZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                CriticalRainfallThreshold = zone.CriticalRainfallThreshold,
                IsActive = zone.IsActive,
                CreatedAt = zone.CreatedAt
            };
        }

        public async Task<bool> DeleteZoneAsync(int id)
        {
            var result = await _zoneRepo.DeleteAsync(id);
            await _zoneRepo.SaveChangesAsync();
            return result;
        }
    }
}