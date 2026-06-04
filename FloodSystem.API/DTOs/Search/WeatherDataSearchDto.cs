namespace FloodSystem.API.DTOs.Search
{
    public class WeatherDataSearchDto
    {
        public int? LocationId { get; set; }
        public decimal? MinRainfall { get; set; }
        public decimal? MaxRainfall { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "RecordedAt";
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}