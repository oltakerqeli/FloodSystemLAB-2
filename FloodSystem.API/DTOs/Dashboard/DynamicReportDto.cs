namespace FloodSystem.API.DTOs.Dashboard;

public class DynamicReportDto
{
    public string ReportType { get; set; } = "all";
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
    public string Format { get; set; } = "excel";
}