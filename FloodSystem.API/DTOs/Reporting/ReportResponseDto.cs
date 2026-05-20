namespace FloodSystem.API.DTOs.Reporting;

public class ReportResponseDto
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}