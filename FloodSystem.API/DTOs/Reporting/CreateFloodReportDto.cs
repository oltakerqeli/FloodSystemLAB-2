namespace FloodSystem.API.DTOs.Reporting;

public class CreateFloodReportDto
{
    public int LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
}