namespace FloodSystem.API.Models.Weather
{
    public class ZoneLocation
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int LocationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Zone Zone { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}