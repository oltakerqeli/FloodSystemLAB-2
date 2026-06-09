using FloodSystem.API.DTOs.Auth;

namespace FloodSystem.API.Services.Auth
{
    public interface IUserService
    {
        Task<object> GetAllUsersAsync();
        Task<object?> GetUserByIdAsync(int id);
        Task<string> DeactivateUserAsync(int id);
        Task<string> AssignRoleAsync(int id, AssignRoleDto dto);
        Task<string> ActivateUserAsync(int id);
    }
}