using System.Data.Common;
using System.Runtime.CompilerServices;
using FloodSystem.API.Models.Auth;
using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Models.Reporting;
using FloodSystem.API.Models.Reporting;

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
                
        }
    }
}