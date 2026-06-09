using FloodSystem.API.Data;
using FloodSystem.API.Models.Reporting;
using FloodSystem.API.DTOs.Reporting;
using FloodSystem.API.Repositories.Reporting;
using FloodSystem.API.MongoDB;

namespace FloodSystem.API.Services.Reporting;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    private readonly MongoDbService _mongo;
    private readonly ApplicationDbContext _context;

    public ReportService(IReportRepository repo, MongoDbService mongo, ApplicationDbContext context)
    {
        _repo = repo;
        _mongo = mongo;
        _context = context;
    }

    public async Task<ReportResponseDto> CreateFloodReportAsync(CreateFloodReportDto dto, int userId)
    {
        var report = new FloodReport
        {
            UserId = userId,
            LocationId = dto.LocationId,
            Description = dto.Description,
            Street = dto.Street,
            District = dto.District,
            Severity = dto.Severity,
            LocationName = dto.LocationName,
            WaterLevelCm = dto.WaterLevelCm,
            StatusId = 1
        };
        var created = await _repo.CreateFloodReportAsync(report);

        await _mongo.GetCollection<ReportActivityLog>("report_activity_logs")
            .InsertOneAsync(new ReportActivityLog
            {
                UserId = userId,
                Action = "CREATE_FLOOD_REPORT",
                Entity = "FloodReport",
                EntityId = created.Id,
                Description = dto.Description
            });

        return new ReportResponseDto
        {
            Id = created.Id,
            LocationId = created.LocationId,
            Description = created.Description,
            Street = created.Street,
            District = created.District,
            Severity = created.Severity,
            LocationName = created.LocationName,
            Status = "Pending",
            ReportType = "Flood",
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<ReportResponseDto> CreateDrainReportAsync(CreateDrainReportDto dto, int userId)
    {
        int? fileId = null;

        if (dto.Photo != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Photo.CopyToAsync(stream);

            var file = new AppFile
            {
                Entity = "DrainReport",
                EntityId = 0,
                Filename = fileName,
                FilePath = filePath,
                FileSize = dto.Photo.Length,
                UploadedBy = userId
            };
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            fileId = file.Id;
        }

        var report = new DrainReport
        {
            UserId = userId,
            LocationId = dto.LocationId,
            Description = dto.Description,
            Street = dto.Street,
            District = dto.District,
            Severity = dto.Severity,
            ReporterName = dto.ReporterName,
            StatusId = 1,
            FileId = fileId
        };

        var created = await _repo.CreateDrainReportAsync(report);

        await _mongo.GetCollection<ReportActivityLog>("report_activity_logs")
            .InsertOneAsync(new ReportActivityLog
            {
                UserId = userId,
                Action = "CREATE_DRAIN_REPORT",
                Entity = "DrainReport",
                EntityId = created.Id,
                Description = dto.Description
            });

        return new ReportResponseDto
        {
            Id = created.Id,
            LocationId = created.LocationId,
            Description = created.Description,
            Street = created.Street,
            District = created.District,
            Severity = created.Severity,
            ReporterName = created.ReporterName,
            Status = "Pending",
            ReportType = "Drain",
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<ReportResponseDto>> GetAllFloodReportsAsync(int userId)
    {
        var reports = await _repo.GetAllFloodReportsAsync(userId);
        return reports.Select(r => new ReportResponseDto
        {
            Id = r.Id,
            LocationId = r.LocationId,
            Description = r.Description,
            Street = r.Street,
            District = r.District,
            Severity = r.Severity,
            LocationName = r.LocationName,
            WaterLevelCm = r.WaterLevelCm,
            Status = r.Status.Name,
            ReportType = "Flood",
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<List<ReportResponseDto>> GetAllDrainReportsAsync(int userId)
    {
        var reports = await _repo.GetAllDrainReportsAsync(userId);
        return reports.Select(r => new ReportResponseDto
        {
            Id = r.Id,
            LocationId = r.LocationId,
            Description = r.Description,
            Street = r.Street,
            District = r.District,
            Severity = r.Severity,
            ReporterName = r.ReporterName,
            Status = r.Status.Name,
            ReportType = "Drain",
            CreatedAt = r.CreatedAt,
            PhotoUrl = r.File != null
                ? $"/uploads/{Path.GetFileName(r.File.FilePath)}"
                : null
        }).ToList();
    }

    public async Task UpdateReportStatusAsync(int id, int statusId, string type)
        => await _repo.UpdateReportStatusAsync(id, statusId, type);

        public async Task<List<ReportResponseDto>> GetAllFloodReportsAsync()
{
    var reports = await _repo.GetAllFloodReportsAsync();
    return reports.Select(r => new ReportResponseDto
    {
        Id = r.Id,
        LocationId = r.LocationId,
        Description = r.Description,
        Street = r.Street,
        District = r.District,
        Severity = r.Severity,
        LocationName = r.LocationName,
        WaterLevelCm = r.WaterLevelCm,
        Status = r.Status.Name,
        ReportType = "Flood",
        CreatedAt = r.CreatedAt
    }).ToList();
}

public async Task<List<ReportResponseDto>> GetAllDrainReportsAsync()
{
    var reports = await _repo.GetAllDrainReportsAsync();
    return reports.Select(r => new ReportResponseDto
    {
        Id = r.Id,
        LocationId = r.LocationId,
        Description = r.Description,
        Street = r.Street,
        District = r.District,
        Severity = r.Severity,
        ReporterName = r.ReporterName,
        Status = r.Status.Name,
        ReportType = "Drain",
        CreatedAt = r.CreatedAt,
        PhotoUrl = r.File != null
            ? $"/uploads/{Path.GetFileName(r.File.FilePath)}"
            : null
    }).ToList();
}
}