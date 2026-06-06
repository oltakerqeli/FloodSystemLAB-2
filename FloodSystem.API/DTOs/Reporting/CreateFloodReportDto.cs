namespace FloodSystem.API.DTOs.Reporting;

public class CreateFloodReportDto
{
    public int LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public decimal WaterLevelCm { get; set; }
}