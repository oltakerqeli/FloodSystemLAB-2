using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Services.Weather
{
    public class LocationService
    {
        private readonly ILocationRepository _locationRepo;

        public LocationService(ILocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        // ✅ NDRYSHO KËTË METODË – Shto parametrin forAdmin
        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync(bool forAdmin = false)
        {
            IEnumerable<Location> locations;
            
            if (forAdmin)
            {
                // Admin/Authority duhet të shohin TË GJITHA lokacionet (edhe të fshirat logjikisht)
                locations = await _locationRepo.GetAllIncludingInactiveAsync();
            }
            else
            {
                // Përdoruesit normal shohin vetëm lokacionet aktive
                locations = await _locationRepo.GetAllAsync();
            }
            
            return locations.Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            var location = await _locationRepo.GetByIdAsync(id);
            if (location == null) return null;

            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt
            };
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto dto)
        {
            var existing = await _locationRepo.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new Exception($"Location '{dto.Name}' already exists");

            var location = new Location
            {
                Name = dto.Name,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _locationRepo.AddAsync(location);
            await _locationRepo.SaveChangesAsync();

            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt
            };
        }

        public async Task<LocationDto?> UpdateLocationAsync(int id, CreateLocationDto dto)
        {
            var location = await _locationRepo.GetByIdAsync(id);
            if (location == null) return null;

            location.Name = dto.Name;
            location.Description = dto.Description;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepo.UpdateAsync(location);
            await _locationRepo.SaveChangesAsync();

            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Description = location.Description,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                IsActive = location.IsActive,
                CreatedAt = location.CreatedAt
            };
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var result = await _locationRepo.DeleteAsync(id);
            await _locationRepo.SaveChangesAsync();
            return result;
        }
    }
}