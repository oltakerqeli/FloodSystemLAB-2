namespace FloodSystem.API.DTOs.Dashboard;

public class ImportDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateImportDto
{
    public string Type { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}