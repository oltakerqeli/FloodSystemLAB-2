using FloodSystem.API.Data;
using FloodSystem.API.Models.Reporting;
using FloodSystem.API.DTOs.Reporting;
using FloodSystem.API.Repositories.Reporting;
using Microsoft.AspNetCore.Hosting;

namespace FloodSystem.API.Services.Reporting;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    private readonly IWebHostEnvironment _env;

    public ReportService(IReportRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    public async Task<ReportResponseDto> CreateFloodReportAsync(CreateFloodReportDto dto, int userId)
    {
        var report = new FloodReport
        {
            UserId = userId,
            LocationId = dto.LocationId,
            Description = dto.Description,
            StatusId = 1 // Pending
        };
        var created = await _repo.CreateFloodReportAsync(report);
        return new ReportResponseDto
        {
            Id = created.Id,
            LocationId = created.LocationId,
            Description = created.Description,
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
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Photo.CopyToAsync(stream);

        }

        var report = new DrainReport
        {
            UserId = userId,
            LocationId = dto.LocationId,
            Description = dto.Description,
            StatusId = 1,
            FileId = fileId
        };

        var created = await _repo.CreateDrainReportAsync(report);
        return new ReportResponseDto
        {
            Id = created.Id,
            LocationId = created.LocationId,
            Description = created.Description,
            Status = "Pending",
            ReportType = "Drain",
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<ReportResponseDto>> GetAllFloodReportsAsync()
    {
        var reports = await _repo.GetAllFloodReportsAsync();
        return reports.Select(r => new ReportResponseDto
        {
            Id = r.Id,
            LocationId = r.LocationId,
            Description = r.Description,
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
            Status = r.Status.Name,
            ReportType = "Drain",
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task UpdateReportStatusAsync(int id, int statusId, string type)
        => await _repo.UpdateReportStatusAsync(id, statusId, type);
}