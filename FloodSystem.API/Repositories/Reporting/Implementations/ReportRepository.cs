using FloodSystem.API.Data;
using FloodSystem.API.Models.Reporting;
using Microsoft.EntityFrameworkCore;
namespace FloodSystem.API.Repositories.Reporting;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FloodReport>> GetAllFloodReportsAsync()
        => await _context.FloodReports.Include(r => r.Status).ToListAsync();

    public async Task<FloodReport?> GetFloodReportByIdAsync(int id)
        => await _context.FloodReports.Include(r => r.Status).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<FloodReport> CreateFloodReportAsync(FloodReport report)
    {
        _context.FloodReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<DrainReport> CreateDrainReportAsync(DrainReport report)
    {
        _context.DrainReports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<List<DrainReport>> GetAllDrainReportsAsync()
    => await _context.DrainReports
        .Include(r => r.Status)
        .Include(r => r.File)
        .ToListAsync();
    public async Task UpdateReportStatusAsync(int id, int statusId, string type)
    {
        if (type == "Flood")
        {
            var report = await _context.FloodReports.FindAsync(id);
            if (report != null) { report.StatusId = statusId; report.UpdatedAt = DateTime.UtcNow; }
        }
        else
        {
            var report = await _context.DrainReports.FindAsync(id);
            if (report != null) { report.StatusId = statusId; report.UpdatedAt = DateTime.UtcNow; }
        }
        await _context.SaveChangesAsync();
    }
}