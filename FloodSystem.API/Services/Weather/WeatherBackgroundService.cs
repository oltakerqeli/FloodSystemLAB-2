using FloodSystem.API.Services.Weather;

namespace FloodSystem.API.Services.Weather
{
    public class WeatherBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WeatherBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

        public WeatherBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<WeatherBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather Background Service started.");

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Fetching weather for all locations...");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
                        await weatherService.FetchAndProcessAllLocationsAsync();
                    }

                    _logger.LogInformation("Weather fetch completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching weather data.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}