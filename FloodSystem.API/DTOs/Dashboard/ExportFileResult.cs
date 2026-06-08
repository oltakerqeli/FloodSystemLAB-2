namespace FloodSystem.API.DTOs.Dashboard;

public class ExportFileResult
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}