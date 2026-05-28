namespace FloodSystem.API.DTOs.Weather
{
    public class SafeRouteDto
    {
        public List<LocationDto> SafeLocations { get; set; } = new();
        public List<LocationDto> CautionLocations { get; set; } = new();
        public List<LocationDto> BlockedLocations { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
    }
}