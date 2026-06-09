using FloodSystem.API.Models.Auth;

namespace FloodSystem.API.Repositories.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddUserAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string tokenHash);
        Task<RefreshToken?> GetActiveRefreshTokenAsync(string tokenHash);
        Task<List<string>> GetUserPermissionsAsync(int userId);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task UpdateUserAsync(User user);
        Task SaveChangesAsync();
    }
}