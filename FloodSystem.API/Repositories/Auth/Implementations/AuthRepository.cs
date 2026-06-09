using FloodSystem.API.Data;
using FloodSystem.API.Models.Auth;
using FloodSystem.API.Repositories.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FloodSystem.API.Repositories.Auth.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt =>
                    rt.TokenHash == tokenHash &&
                    rt.RevokedAt == null &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<RefreshToken?> GetActiveRefreshTokenAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.TokenHash == tokenHash &&
                    rt.RevokedAt == null);
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}