using System.Text.Json;
using FloodSystem.API.Data;
using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Models.Weather;
using FloodSystem.API.Repositories.Weather.Interfaces;
using FloodSystem.API.MongoDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace FloodSystem.API.Services.Weather
{
    public class WeatherService
    {
        private readonly IWeatherDataRepository _weatherDataRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly IAlertRepository _alertRepo;
        private readonly ITrafficUpdateRepository _trafficRepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;
        private readonly HttpClient _httpClient;
        private readonly MongoDbService _mongoDbService;

        public WeatherService(
            IWeatherDataRepository weatherDataRepo,
            ILocationRepository locationRepo,
            IAlertRepository alertRepo,
            ITrafficUpdateRepository trafficRepo,
            IConfiguration configuration,
            ILogger<WeatherService> logger,
            MongoDbService mongoDbService) 
        {
            _weatherDataRepo = weatherDataRepo;
            _locationRepo = locationRepo;
            _alertRepo = alertRepo;
            _trafficRepo = trafficRepo;
            _mongoDbService = mongoDbService;
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        private string GetRiskLevel(decimal rainfall)
        {
            if (rainfall > 10) return "HIGH";
            if (rainfall > 5) return "MEDIUM";
            return "LOW";
        }

        private string GetTrafficStatus(string riskLevel)
        {
            return riskLevel switch
            {
                "HIGH" => "BLOCKED",
                "MEDIUM" => "CAUTION",
                _ => "OPEN"
            };
        }

        private string GetAlertMessage(string locationName, decimal rainfall, string riskLevel)
        {
            return riskLevel switch
            {
                "HIGH" => $"🚨 HIGH FLOOD RISK at {locationName}! Rainfall: {rainfall}mm. Road BLOCKED.",
                "MEDIUM" => $"⚠️ MEDIUM FLOOD RISK at {locationName}. Rainfall: {rainfall}mm. Use CAUTION.",
                _ => $"✅ Low flood risk at {locationName}. Rainfall: {rainfall}mm. Status OPEN."
            };
        }

        public async Task<WeatherDataDto?> FetchAndProcessWeatherAsync(int locationId)
        {
            var location = await _locationRepo.GetByIdAsync(locationId);
            if (location == null)
                throw new Exception($"Location {locationId} not found");

            var apiKey = _configuration["OpenWeatherMap:ApiKey"];
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var rainfall = 0m;
            if (root.TryGetProperty("rain", out var rain))
            {
                if (rain.TryGetProperty("1h", out var oneHour))
                    rainfall = oneHour.GetDecimal();
            }

            var weatherData = new WeatherData
            {
                LocationId = locationId,
                Temperature = root.GetProperty("main").GetProperty("temp").GetDecimal(),
                Rainfall = rainfall,
                Humidity = root.GetProperty("main").GetProperty("humidity").GetDecimal(),
                RecordedAt = DateTime.UtcNow
            };

            await _weatherDataRepo.AddAsync(weatherData);
            try
{
    var rawCollection = _mongoDbService.GetCollection<WeatherRawData>("weather_raw");
    var rawDoc = new WeatherRawData
    {
        LocationId = locationId,
        LocationName = location.Name,
        Temperature = weatherData.Temperature,
        Rainfall = weatherData.Rainfall,
        Humidity = weatherData.Humidity,
        WeatherCondition = "Unknown",
        WindSpeed = 0,
        Pressure = 0,
        FetchedAt = DateTime.UtcNow,
        Source = "OpenWeatherMap"
    };
    await rawCollection.InsertOneAsync(rawDoc);
    _logger.LogInformation($"Raw weather data saved to MongoDB for {location.Name}");
}
catch (Exception ex)
{
    _logger.LogWarning($"Could not save to MongoDB: {ex.Message}");
}

            var riskLevel = GetRiskLevel(rainfall);
            var alert = new Alert
            {
                Type = "Flood Warning",
                Message = GetAlertMessage(location.Name, rainfall, riskLevel),
                RiskLevel = riskLevel,
                LocationId = locationId,
                CreatedAt = DateTime.UtcNow
            };
            await _alertRepo.AddAsync(alert);

            var trafficUpdate = new TrafficUpdate
            {
                LocationId = locationId,
                Status = GetTrafficStatus(riskLevel),
                Description = $"Road {GetTrafficStatus(riskLevel)} - {riskLevel} flood risk",
                CreatedAt = DateTime.UtcNow
            };
            await _trafficRepo.AddAsync(trafficUpdate);

            await _weatherDataRepo.SaveChangesAsync();

            _logger.LogInformation($"Weather processed for {location.Name}: {rainfall}mm -> {riskLevel} risk");

            return new WeatherDataDto
            {
                Id = weatherData.Id,
                LocationId = locationId,
                LocationName = location.Name,
                Temperature = weatherData.Temperature,
                Rainfall = weatherData.Rainfall,
                Humidity = weatherData.Humidity,
                RecordedAt = weatherData.RecordedAt,
                RiskLevel = riskLevel
            };
        }

        public async Task FetchAndProcessAllLocationsAsync()
        {
            var locations = await _locationRepo.GetActiveLocationsAsync();
            foreach (var location in locations)
            {
                try
                {
                    await FetchAndProcessWeatherAsync(location.Id);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to fetch weather for {location.Name}: {ex.Message}");
                }
            }
        }

        public async Task<WeatherDataDto?> GetLatestWeatherAsync(int locationId)
        {
            var data = await _weatherDataRepo.GetLatestByLocationIdAsync(locationId);
            if (data == null) return null;

            var location = await _locationRepo.GetByIdAsync(locationId);

            return new WeatherDataDto
            {
                Id = data.Id,
                LocationId = data.LocationId,
                LocationName = location?.Name ?? "",
                Temperature = data.Temperature,
                Rainfall = data.Rainfall,
                Humidity = data.Humidity,
                RecordedAt = data.RecordedAt,
                RiskLevel = GetRiskLevel(data.Rainfall)
            };
        }

        public async Task<IEnumerable<WeatherDataDto>> GetWeatherHistoryAsync(int locationId)
        {
            var history = await _weatherDataRepo.GetByLocationIdAsync(locationId);
            var location = await _locationRepo.GetByIdAsync(locationId);

            return history.Select(data => new WeatherDataDto
            {
                Id = data.Id,
                LocationId = data.LocationId,
                LocationName = location?.Name ?? "",
                Temperature = data.Temperature,
                Rainfall = data.Rainfall,
                Humidity = data.Humidity,
                RecordedAt = data.RecordedAt,
                RiskLevel = GetRiskLevel(data.Rainfall)
            });
        }

        public async Task<string> GetRiskLevelForLocationAsync(int locationId)
        {
            var latest = await _weatherDataRepo.GetLatestByLocationIdAsync(locationId);
            return latest == null ? "UNKNOWN" : GetRiskLevel(latest.Rainfall);
        }
    }
}