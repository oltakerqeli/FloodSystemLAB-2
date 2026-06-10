using System.Data.Common;
using System.Runtime.CompilerServices;
using FloodSystem.API.Models.Auth;
using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Models.Reporting;
using FloodSystem.API.Models.Dashboard;
using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<FloodReport> FloodReports => Set<FloodReport>();
        public DbSet<DrainReport> DrainReports => Set<DrainReport>();
        public DbSet<ReportStatus> ReportStatuses => Set<ReportStatus>();
        public DbSet<ReportType> ReportTypes => Set<ReportType>();
        public DbSet<AppFile> Files => Set<AppFile>();
        public DbSet<UserReport> UserReports => Set<UserReport>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<ReportLog> ReportLogs => Set<ReportLog>();
        public DbSet<Export> Exports => Set<Export>();
        public DbSet<Import> Imports => Set<Import>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Zone> Zones => Set<Zone>();
        public DbSet<ZoneLocation> ZoneLocations => Set<ZoneLocation>();
        public DbSet<WeatherData> WeatherData => Set<WeatherData>();
        public DbSet<Alert> Alerts => Set<Alert>();
        public DbSet<TrafficUpdate> TrafficUpdates => Set<TrafficUpdate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "System administrator",
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Role
                {
                    Id = 2,
                    Name = "User",
                    Description = "Regular system user",
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Role
                {
                    Id = 3,
                    Name = "Authority",
                    Description = "Municipality or emergency authority user",
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );
            modelBuilder.Entity<ReportStatus>().HasData(
                new ReportStatus { Id = 1, Name = "Pending" },
                new ReportStatus { Id = 2, Name = "In Progress" },
                new ReportStatus { Id = 3, Name = "Resolved" }
            );

            modelBuilder.Entity<ReportType>().HasData(
                new ReportType { Id = 1, Name = "Flood" },
                new ReportType { Id = 2, Name = "Drain" },
                new ReportType { Id = 3, Name = "Other" }
            );
            modelBuilder.Entity<Setting>().HasData(
                new Setting { Id = 1, Key = "AppName", Value = "Flood System", Description = "Application name", UpdatedAt = new DateTime(2026, 1, 1) },
                new Setting { Id = 2, Key = "MaxAlertLevel", Value = "3", Description = "Maximum alert risk level", UpdatedAt = new DateTime(2026, 1, 1) },
                new Setting { Id = 3, Key = "WeatherFetchInterval", Value = "30", Description = "Weather API fetch interval in minutes", UpdatedAt = new DateTime(2026, 1, 1) }
            );

            modelBuilder.Entity<Location>(entity =>
           {
               entity.HasKey(e => e.Id);
               entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
               entity.Property(e => e.Description).HasMaxLength(500);
               entity.Property(e => e.Latitude).HasPrecision(9, 6);
               entity.Property(e => e.Longitude).HasPrecision(9, 6);
               entity.HasIndex(e => e.Name).IsUnique();
           });
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CriticalRainfallThreshold).HasPrecision(5, 2);
            });

            // ZoneLocation (Many-to-Mmany lidhja)
            modelBuilder.Entity<ZoneLocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Zone)
                    .WithMany(z => z.ZoneLocations)
                    .HasForeignKey(e => e.ZoneId);
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.ZoneLocations)
                    .HasForeignKey(e => e.LocationId);
            });

            // WeatherData konfigurimi
            modelBuilder.Entity<WeatherData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Temperature).HasPrecision(5, 2);
                entity.Property(e => e.Rainfall).HasPrecision(5, 2);
                entity.Property(e => e.Humidity).HasPrecision(5, 2);
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.WeatherData)
                    .HasForeignKey(e => e.LocationId);
            });

            // Alerts konfigurimi
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.RiskLevel).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Alerts)
                    .HasForeignKey(e => e.LocationId);
            });
            modelBuilder.Entity<FloodReport>()
    .Property(r => r.WaterLevelCm)
    .HasPrecision(8, 2);

            // TrafficUpdates konfigurimi
            modelBuilder.Entity<TrafficUpdate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.TrafficUpdates)
                    .HasForeignKey(e => e.LocationId);
            });
            modelBuilder.Entity<AppFile>()
                .HasOne(f => f.UploadedByUser)
                .WithMany()
                .HasForeignKey(f => f.UploadedBy);

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "ManageUsers", Description = "Can manage system users" },
                new Permission { Id = 2, Name = "ManageRoles", Description = "Can assign and manage roles" },
                new Permission { Id = 3, Name = "ViewDashboard", Description = "Can view dashboard" },
                new Permission { Id = 4, Name = "CreateReport", Description = "Can create reports" },
                new Permission { Id = 5, Name = "ViewReports", Description = "Can view reports" },
                new Permission { Id = 6, Name = "ManageReports", Description = "Can manage reports" },
                new Permission { Id = 7, Name = "ManageLocations", Description = "Can manage locations and zones" },
                new Permission { Id = 8, Name = "ManageAlerts", Description = "Can manage alerts" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                // Admin - all permissions
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1 },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2 },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3 },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4 },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5 },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6 },
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7 },
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8 },

                // User
                new RolePermission { Id = 9, RoleId = 2, PermissionId = 4 },
                new RolePermission { Id = 10, RoleId = 2, PermissionId = 5 },

                // Authority
                new RolePermission { Id = 11, RoleId = 3, PermissionId = 3 },
                new RolePermission { Id = 12, RoleId = 3, PermissionId = 5 },
                new RolePermission { Id = 13, RoleId = 3, PermissionId = 6 },
                new RolePermission { Id = 14, RoleId = 3, PermissionId = 7 },
                new RolePermission { Id = 15, RoleId = 3, PermissionId = 8 }
            );

        }
    }
}