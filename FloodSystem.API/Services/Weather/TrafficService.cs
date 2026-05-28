using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Services.Weather
{
    public class TrafficService
    {
        private readonly ITrafficUpdateRepository _trafficRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly IWeatherDataRepository _weatherRepo;

        public TrafficService(
            ITrafficUpdateRepository trafficRepo,
            ILocationRepository locationRepo,
            IWeatherDataRepository weatherRepo)
        {
            _trafficRepo = trafficRepo;
            _locationRepo = locationRepo;
            _weatherRepo = weatherRepo;
        }

        private string GetRiskLevel(decimal rainfall)
        {
            if (rainfall > 10) return "HIGH";
            if (rainfall > 5) return "MEDIUM";
            return "LOW";
        }

        public async Task<TrafficUpdateDto?> GetLatestTrafficAsync(int locationId)
        {
            var traffic = await _trafficRepo.GetLatestByLocationIdAsync(locationId);
            if (traffic == null) return null;

            return new TrafficUpdateDto
            {
                Id = traffic.Id,
                LocationId = traffic.LocationId,
                LocationName = traffic.Location?.Name ?? "",
                Status = traffic.Status,
                Description = traffic.Description,
                CreatedAt = traffic.CreatedAt
            };
        }

        public async Task<IEnumerable<TrafficUpdateDto>> GetTrafficHistoryAsync(int locationId)
        {
            var history = await _trafficRepo.GetByLocationIdAsync(locationId);
            return history.Select(t => new TrafficUpdateDto
            {
                Id = t.Id,
                LocationId = t.LocationId,
                LocationName = t.Location?.Name ?? "",
                Status = t.Status,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            });
        }

        public async Task<SafeRouteDto> GetSafeRoutesAsync()
        {
            var allLocations = await _locationRepo.GetAllAsync();
            var safeLocations = new List<LocationDto>();
            var cautionLocations = new List<LocationDto>();
            var blockedLocations = new List<LocationDto>();

            foreach (var location in allLocations)
            {
                var latestWeather = await _weatherRepo.GetLatestByLocationIdAsync(location.Id);
                var riskLevel = latestWeather != null ? GetRiskLevel(latestWeather.Rainfall) : "LOW";

                var locationDto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Description = location.Description,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    IsActive = location.IsActive,
                    CreatedAt = location.CreatedAt
                };

                switch (riskLevel)
                {
                    case "HIGH":
                        blockedLocations.Add(locationDto);
                        break;
                    case "MEDIUM":
                        cautionLocations.Add(locationDto);
                        break;
                    default:
                        safeLocations.Add(locationDto);
                        break;
                }
            }

            return new SafeRouteDto
            {
                SafeLocations = safeLocations,
                CautionLocations = cautionLocations,
                BlockedLocations = blockedLocations,
                Summary = $"{safeLocations.Count} safe, {cautionLocations.Count} caution, {blockedLocations.Count} blocked"
            };
        }
    }
}