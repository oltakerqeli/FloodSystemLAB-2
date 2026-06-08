using FloodSystem.API.DTOs.Dashboard;
using FloodSystem.API.Models.Dashboard;
using FloodSystem.API.Repositories.Dashboard;
using ClosedXML.Excel;
using FloodSystem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FloodSystem.API.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;
    private readonly ApplicationDbContext _context;

    public DashboardService(IDashboardRepository repo, ApplicationDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<List<AuditLogDto>> GetAllAuditLogsAsync()
    {
        var logs = await _repo.GetAllAuditLogsAsync();
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Action = l.Action,
            Entity = l.Entity,
            EntityId = l.EntityId,
            OldValue = l.OldValue,
            NewValue = l.NewValue,
            IpAddress = l.IpAddress,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsByUserAsync(int userId)
    {
        var notifications = await _repo.GetNotificationsByUserAsync(userId);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            UserId = dto.UserId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message
        };
        await _repo.CreateNotificationAsync(notification);
    }

    public async Task MarkNotificationAsReadAsync(int id)
        => await _repo.MarkNotificationAsReadAsync(id);

    public async Task<List<SettingDto>> GetAllSettingsAsync()
    {
        var settings = await _repo.GetAllSettingsAsync();
        return settings.Select(s => new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description
        }).ToList();
    }

    public async Task UpdateSettingAsync(int id, string value)
        => await _repo.UpdateSettingAsync(id, value);

    public async Task<ImportDto> ImportDataAsync(IFormFile file, string type, int userId)
    {
        var import = new Import { UserId = userId, Type = type, Format = Path.GetExtension(file.FileName).TrimStart('.') };
        await _repo.CreateImportAsync(import);

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension == ".csv")
        {
            using var reader = new System.IO.StreamReader(file.OpenReadStream());
            var allText = await reader.ReadToEndAsync();
            var lines = allText.Split('\n').Select(l => l.TrimEnd('\r')).ToList();

            if (type.ToLower() == "flood")
            {
                foreach (var csvLine in lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("===")))
                {
                    var cols = csvLine.Split(',');
                    if (cols.Length < 6) continue;
                    _context.FloodReports.Add(new FloodSystem.API.Models.Reporting.FloodReport
                    {
                        UserId = userId, LocationId = 1,
                        Description = cols[0].Trim('"'), Street = cols[1].Trim('"'),
                        District = cols[2].Trim('"'), Severity = cols[3].Trim('"'), StatusId = 1
                    });
                }
            }
            else if (type.ToLower() == "drain")
            {
                foreach (var csvLine in lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("===")))
                {
                    var cols = csvLine.Split(',');
                    if (cols.Length < 6) continue;
                    _context.DrainReports.Add(new FloodSystem.API.Models.Reporting.DrainReport
                    {
                        UserId = userId, LocationId = 1,
                        Description = cols[0].Trim('"'), Street = cols[1].Trim('"'),
                        District = cols[2].Trim('"'), Severity = cols[3].Trim('"'), StatusId = 1
                    });
                }
            }
            else if (type.ToLower() == "locations")
            {
                foreach (var csvLine in lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("===")))
                {
                    var cols = csvLine.Split(',');
                    if (cols.Length < 4) continue;
                    _context.Locations.Add(new FloodSystem.API.Models.Weather.Location
                    {
                        Name = cols[0].Trim('"'),
                        Latitude = decimal.TryParse(cols[1], out var lat) ? lat : 0,
                        Longitude = decimal.TryParse(cols[2], out var lng) ? lng : 0,
                        IsActive = true
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        else if (extension == ".xlsx")
        {
            using var workbook = new XLWorkbook(file.OpenReadStream());

            if (type.ToLower() == "flood" && workbook.Worksheets.Contains("Flood Reports"))
            {
                var ws = workbook.Worksheet("Flood Reports");
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    _context.FloodReports.Add(new FloodSystem.API.Models.Reporting.FloodReport
                    {
                        UserId = userId, LocationId = 1,
                        Description = row.Cell(2).GetString(), Street = row.Cell(3).GetString(),
                        District = row.Cell(4).GetString(), Severity = row.Cell(5).GetString(), StatusId = 1
                    });
                }
            }
            else if (type.ToLower() == "drain" && workbook.Worksheets.Contains("Drain Reports"))
            {
                var ws = workbook.Worksheet("Drain Reports");
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    _context.DrainReports.Add(new FloodSystem.API.Models.Reporting.DrainReport
                    {
                        UserId = userId, LocationId = 1,
                        Description = row.Cell(2).GetString(), Street = row.Cell(3).GetString(),
                        District = row.Cell(4).GetString(), Severity = row.Cell(5).GetString(), StatusId = 1
                    });
                }
            }
            else if (type.ToLower() == "locations" && workbook.Worksheets.Contains("Locations"))
            {
                var ws = workbook.Worksheet("Locations");
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    _context.Locations.Add(new FloodSystem.API.Models.Weather.Location
                    {
                        Name = row.Cell(2).GetString(),
                        Latitude = decimal.TryParse(row.Cell(3).GetString(), out var lat) ? lat : 0,
                        Longitude = decimal.TryParse(row.Cell(4).GetString(), out var lng) ? lng : 0,
                        IsActive = true
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        else if (extension == ".json")
        {
            using var reader = new System.IO.StreamReader(file.OpenReadStream());
            var json = await reader.ReadToEndAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (type.ToLower() == "flood")
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<List<FloodImportDto>>(json, options);
                if (data != null)
                    foreach (var r in data)
                        _context.FloodReports.Add(new FloodSystem.API.Models.Reporting.FloodReport
                        {
                            UserId = userId, LocationId = 1,
                            Description = r.Description, Street = r.Street,
                            District = r.District, Severity = r.Severity, StatusId = 1
                        });
            }
            else if (type.ToLower() == "drain")
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<List<DrainImportDto>>(json, options);
                if (data != null)
                    foreach (var r in data)
                        _context.DrainReports.Add(new FloodSystem.API.Models.Reporting.DrainReport
                        {
                            UserId = userId, LocationId = 1,
                            Description = r.Description, Street = r.Street,
                            District = r.District, Severity = r.Severity, StatusId = 1
                        });
            }
            else if (type.ToLower() == "locations")
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<List<LocationImportDto>>(json, options);
                if (data != null)
                    foreach (var l in data)
                        _context.Locations.Add(new FloodSystem.API.Models.Weather.Location
                        {
                            Name = l.Name, Latitude = l.Latitude,
                            Longitude = l.Longitude, IsActive = true
                        });
            }

            await _context.SaveChangesAsync();
        }

        return new ImportDto { Id = import.Id, Type = import.Type, Format = import.Format, CreatedAt = import.CreatedAt };
    }

    public async Task<ExportFileResult> ExportReportsAsync(CreateExportDto dto, int userId)
    {
        var export = new Export { UserId = userId, Type = dto.Type, Format = dto.Format };
        await _repo.CreateExportAsync(export);

        var format = dto.Format.ToLower();
        var type = dto.Type.ToLower();

        var floodReports = (type == "all" || type == "flood" || type == "reports")
            ? await _context.FloodReports.Include(r => r.Status).Where(r => r.UserId == userId).ToListAsync()
            : new List<FloodSystem.API.Models.Reporting.FloodReport>();

        var drainReports = (type == "all" || type == "drain" || type == "reports")
            ? await _context.DrainReports.Include(r => r.Status).Where(r => r.UserId == userId).ToListAsync()
            : new List<FloodSystem.API.Models.Reporting.DrainReport>();

        var alerts = (type == "all" || type == "alerts")
            ? await _context.Alerts.ToListAsync()
            : new List<FloodSystem.API.Models.Weather.Alert>();

        var locations = (type == "all" || type == "locations")
            ? await _context.Locations.ToListAsync()
            : new List<FloodSystem.API.Models.Weather.Location>();

        if (format == "csv")
        {
            var sb = new System.Text.StringBuilder();

            if (floodReports.Any())
            {
                sb.AppendLine("=== FLOOD REPORTS ===");
                sb.AppendLine("Id,Description,Street,District,Severity,Status,CreatedAt");
                foreach (var r in floodReports)
                    sb.AppendLine($"{r.Id},\"{r.Description}\",\"{r.Street}\",\"{r.District}\",{r.Severity},{r.Status.Name},{r.CreatedAt:yyyy-MM-dd}");
                sb.AppendLine();
            }

            if (drainReports.Any())
            {
                sb.AppendLine("=== DRAIN REPORTS ===");
                sb.AppendLine("Id,Description,Street,District,Severity,Status,CreatedAt");
                foreach (var r in drainReports)
                    sb.AppendLine($"{r.Id},\"{r.Description}\",\"{r.Street}\",\"{r.District}\",{r.Severity},{r.Status.Name},{r.CreatedAt:yyyy-MM-dd}");
                sb.AppendLine();
            }

            if (alerts.Any())
            {
                sb.AppendLine("=== ALERTS ===");
                sb.AppendLine("Id,Type,Message,RiskLevel,CreatedAt");
                foreach (var a in alerts)
                    sb.AppendLine($"{a.Id},{a.Type},\"{a.Message}\",{a.RiskLevel},{a.CreatedAt:yyyy-MM-dd}");
                sb.AppendLine();
            }

            if (locations.Any())
            {
                sb.AppendLine("=== LOCATIONS ===");
                sb.AppendLine("Id,Name,Latitude,Longitude,IsActive");
                foreach (var l in locations)
                    sb.AppendLine($"{l.Id},\"{l.Name}\",{l.Latitude},{l.Longitude},{l.IsActive}");
            }

            return new ExportFileResult
            {
                Content = System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
                ContentType = "text/csv",
                FileName = $"{type}_export_{DateTime.UtcNow:yyyyMMdd}.csv"
            };
        }
        else if (format == "excel")
        {
            using var workbook = new XLWorkbook();

            if (floodReports.Any())
            {
                var ws = workbook.Worksheets.Add("Flood Reports");
                AddExcelHeaders(ws, new[] { "Id", "Description", "Street", "District", "Severity", "Status", "Created At" }, "#1e3a5f");
                var row = 2;
                foreach (var r in floodReports)
                {
                    ws.Cell(row, 1).Value = r.Id;
                    ws.Cell(row, 2).Value = r.Description;
                    ws.Cell(row, 3).Value = r.Street;
                    ws.Cell(row, 4).Value = r.District;
                    ws.Cell(row, 5).Value = r.Severity;
                    ws.Cell(row, 6).Value = r.Status.Name;
                    ws.Cell(row, 7).Value = r.CreatedAt.ToString("yyyy-MM-dd");
                    StyleExcelRow(ws, row, 7, row % 2 == 0 ? "#dbeafe" : "#ffffff");
                    row++;
                }
                ws.Columns().AdjustToContents();
            }

            if (drainReports.Any())
            {
                var ws = workbook.Worksheets.Add("Drain Reports");
                AddExcelHeaders(ws, new[] { "Id", "Description", "Street", "District", "Severity", "Status", "Created At" }, "#1e3a5f");
                var row = 2;
                foreach (var r in drainReports)
                {
                    ws.Cell(row, 1).Value = r.Id;
                    ws.Cell(row, 2).Value = r.Description;
                    ws.Cell(row, 3).Value = r.Street;
                    ws.Cell(row, 4).Value = r.District;
                    ws.Cell(row, 5).Value = r.Severity;
                    ws.Cell(row, 6).Value = r.Status.Name;
                    ws.Cell(row, 7).Value = r.CreatedAt.ToString("yyyy-MM-dd");
                    StyleExcelRow(ws, row, 7, row % 2 == 0 ? "#fef9c3" : "#ffffff");
                    row++;
                }
                ws.Columns().AdjustToContents();
            }

            if (alerts.Any())
            {
                var ws = workbook.Worksheets.Add("Alerts");
                AddExcelHeaders(ws, new[] { "Id", "Type", "Message", "Risk Level", "Created At" }, "#7f1d1d");
                var row = 2;
                foreach (var a in alerts)
                {
                    ws.Cell(row, 1).Value = a.Id;
                    ws.Cell(row, 2).Value = a.Type;
                    ws.Cell(row, 3).Value = a.Message;
                    ws.Cell(row, 4).Value = a.RiskLevel;
                    ws.Cell(row, 5).Value = a.CreatedAt.ToString("yyyy-MM-dd");
                    StyleExcelRow(ws, row, 5, row % 2 == 0 ? "#fee2e2" : "#ffffff");
                    row++;
                }
                ws.Columns().AdjustToContents();
            }

            if (locations.Any())
            {
                var ws = workbook.Worksheets.Add("Locations");
                AddExcelHeaders(ws, new[] { "Id", "Name", "Latitude", "Longitude", "Active" }, "#064e3b");
                var row = 2;
                foreach (var l in locations)
                {
                    ws.Cell(row, 1).Value = l.Id;
                    ws.Cell(row, 2).Value = l.Name;
                    ws.Cell(row, 3).Value = (double)l.Latitude;
                    ws.Cell(row, 4).Value = (double)l.Longitude;
                    ws.Cell(row, 5).Value = l.IsActive ? "Yes" : "No";
                    StyleExcelRow(ws, row, 5, row % 2 == 0 ? "#d1fae5" : "#ffffff");
                    row++;
                }
                ws.Columns().AdjustToContents();
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return new ExportFileResult
            {
                Content = ms.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"{type}_export_{DateTime.UtcNow:yyyyMMdd}.xlsx"
            };
        }
        else
        {
            var data = new
            {
                exportedAt = DateTime.UtcNow,
                type = dto.Type,
                floodReports = floodReports.Any() ? floodReports.Select(r => new {
                    r.Id, r.Description, r.Street, r.District,
                    r.Severity, status = r.Status.Name, r.CreatedAt
                }) : null,
                drainReports = drainReports.Any() ? drainReports.Select(r => new {
                    r.Id, r.Description, r.Street, r.District,
                    r.Severity, status = r.Status.Name, r.CreatedAt
                }) : null,
                alerts = alerts.Any() ? alerts.Select(a => new {
                    a.Id, a.Type, a.Message, a.RiskLevel, a.CreatedAt
                }) : null,
                locations = locations.Any() ? locations.Select(l => new {
                    l.Id, l.Name, l.Latitude, l.Longitude, l.IsActive
                }) : null
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            return new ExportFileResult
            {
                Content = System.Text.Encoding.UTF8.GetBytes(json),
                ContentType = "application/json",
                FileName = $"{type}_export_{DateTime.UtcNow:yyyyMMdd}.json"
            };
        }
    }

    private void AddExcelHeaders(IXLWorksheet ws, string[] headers, string colorHex)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(colorHex);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
    }

    private void StyleExcelRow(IXLWorksheet ws, int row, int colCount, string colorHex)
    {
        var range = ws.Range(row, 1, row, colCount);
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(colorHex);
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    }

    public async Task<ExportDto> CreateExportAsync(CreateExportDto dto, int userId)
    {
        var export = new Export { UserId = userId, Type = dto.Type, Format = dto.Format };
        var created = await _repo.CreateExportAsync(export);
        return new ExportDto { Id = created.Id, Type = created.Type, Format = created.Format, CreatedAt = created.CreatedAt };
    }
}