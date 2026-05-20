namespace FloodSystem.API.DTOs.Reporting;

public class CreateDrainReportDto
{
    public int LocationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public IFormFile? Photo { get; set; }
}