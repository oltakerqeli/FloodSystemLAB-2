using FloodSystem.API.DTOs.Auth;
using FloodSystem.API.Models.Auth;
using FloodSystem.API.Repositories.Auth.Interfaces;

namespace FloodSystem.API.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<object> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.Name)
            }).ToList();
        }

        public async Task<object?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return null;

            return new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name)
            };
        }

        public async Task<string> DeactivateUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdSimpleAsync(id);

            if (user == null)
                return "User not found.";

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return "User deactivated successfully.";
        }

        public async Task<string> ActivateUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdSimpleAsync(id);

            if (user == null)
                return "User not found.";

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return "User activated successfully.";
        }

        public async Task<string> AssignRoleAsync(int id, AssignRoleDto dto)
        {
            var user = await _userRepository.GetUserByIdSimpleAsync(id);

            if (user == null)
                return "User not found.";

            var role = await _userRepository.GetRoleByNameAsync(dto.RoleName);

            if (role == null)
                return "Role not found.";

            var userRoles = await _userRepository.GetUserRolesAsync(id);

            if (userRoles.Count == 1 && userRoles.Any(ur => ur.RoleId == role.Id))
                return "User already has this role.";

            _userRepository.RemoveUserRoles(userRoles);

            await _userRepository.AddUserRoleAsync(new UserRole
            {
                UserId = id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            });

            await _userRepository.SaveChangesAsync();

            return $"User role changed to {role.Name}.";
        }
    }
}