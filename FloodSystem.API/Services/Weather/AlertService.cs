using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;

namespace FloodSystem.API.Services.Weather
{
    public class AlertService
    {
        private readonly IAlertRepository _alertRepo;
        private readonly ILocationRepository _locationRepo;

        public AlertService(IAlertRepository alertRepo, ILocationRepository locationRepo)
        {
            _alertRepo = alertRepo;
            _locationRepo = locationRepo;
        }

        public async Task<IEnumerable<AlertDto>> GetAllAlertsAsync()
        {
            var alerts = await _alertRepo.GetAllAsync();
            return alerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type,
                Message = a.Message,
                RiskLevel = a.RiskLevel,
                LocationId = a.LocationId,
                LocationName = a.Location?.Name ?? "",
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<IEnumerable<AlertDto>> GetActiveAlertsAsync()
        {
            var alerts = await _alertRepo.GetAllAsync();
            var highRiskAlerts = alerts.Where(a => a.RiskLevel == "HIGH");

            return highRiskAlerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type,
                Message = a.Message,
                RiskLevel = a.RiskLevel,
                LocationId = a.LocationId,
                LocationName = a.Location?.Name ?? "",
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsByLocationAsync(int locationId)
        {
            var alerts = await _alertRepo.GetByLocationIdAsync(locationId);
            return alerts.Select(a => new AlertDto
            {
                Id = a.Id,
                Type = a.Type,
                Message = a.Message,
                RiskLevel = a.RiskLevel,
                LocationId = a.LocationId,
                LocationName = a.Location?.Name ?? "",
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<AlertDto?> GetAlertByIdAsync(int id)
        {
            var alert = await _alertRepo.GetByIdAsync(id);
            if (alert == null) return null;

            return new AlertDto
            {
                Id = alert.Id,
                Type = alert.Type,
                Message = alert.Message,
                RiskLevel = alert.RiskLevel,
                LocationId = alert.LocationId,
                LocationName = alert.Location?.Name ?? "",
                CreatedAt = alert.CreatedAt
            };
        }
    }
}