namespace FloodSystem.API.DTOs.Weather
{
    public class WeatherDataDto
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public decimal Temperature { get; set; }
        public decimal Rainfall { get; set; }
        public decimal Humidity { get; set; }
        public DateTime RecordedAt { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
    }
}