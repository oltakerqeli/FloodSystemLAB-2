namespace FloodSystem.API.DTOs.Reporting;

public class ReportResponseDto
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Street { get; set; }
    public string? District { get; set; }
    public string? Severity { get; set; }
    public string? LocationName { get; set; }
    public string? ReporterName { get; set; }
    public decimal WaterLevelCm { get; set; }
}