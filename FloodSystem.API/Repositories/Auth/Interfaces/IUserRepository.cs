using FloodSystem.API.Models.Auth;

namespace FloodSystem.API.Repositories.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByIdSimpleAsync(int id);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<UserRole?> GetUserRoleAsync(int userId, int roleId);
        Task AddUserRoleAsync(UserRole userRole);
        Task<List<UserRole>> GetUserRolesAsync(int userId);
        void RemoveUserRoles(List<UserRole> userRoles);
        Task SaveChangesAsync();
    }
}