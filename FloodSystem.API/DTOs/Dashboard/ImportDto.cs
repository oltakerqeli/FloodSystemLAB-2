namespace FloodSystem.API.DTOs.Dashboard;

public class ImportDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
public class FloodImportDto
{
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

public class DrainImportDto
{
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

public class LocationImportDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}
public class CreateImportDto
{
    public string Type { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}