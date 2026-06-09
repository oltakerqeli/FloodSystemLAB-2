using FloodSystem.API.DTOs.Auth;
using FloodSystem.API.Models.Auth;
using FloodSystem.API.Repositories.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FloodSystem.API.MongoDB;
using FloodSystem.API.Services.Interfaces;

namespace FloodSystem.API.Services.Auth
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly MongoDbService _mongoDbService;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, MongoDbService mongoDbService, IEmailService emailService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mongoDbService = mongoDbService;
            _emailService = emailService;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null)
                return "Email already exists.";

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.AddUserAsync(user);
            await _authRepository.SaveChangesAsync();

            var userRole = await _authRepository.GetRoleByNameAsync("User");

            if (userRole != null)
            {
                await _authRepository.AddUserRoleAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow
                });

                await _authRepository.SaveChangesAsync();
            }

            return "User registered successfully.";
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                await LogLoginActivityAsync(null, dto.Email, "LOGIN_FAILED");
                return null;
            }

            if (!user.IsActive)
            {
                throw new Exception("Your account has been deactivated.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                await LogLoginActivityAsync(user.Id, user.Email, "LOGIN_FAILED");
                return null;
            }

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashRefreshToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            await _authRepository.SaveChangesAsync();

            await LogLoginActivityAsync(user.Id, user.Email, "LOGIN_SUCCESS");

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto?> RefreshAsync(string refreshToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);

            var storedToken = await _authRepository.GetValidRefreshTokenAsync(tokenHash);

            if (storedToken == null)
                return null;

            storedToken.RevokedAt = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();

            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                UserId = storedToken.UserId,
                TokenHash = HashRefreshToken(newRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            var newAccessToken = await GenerateJwtTokenAsync(storedToken.User);

            await _authRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);

            var storedToken = await _authRepository.GetActiveRefreshTokenAsync(tokenHash);

            if (storedToken == null)
                return false;

            storedToken.RevokedAt = DateTime.UtcNow;

            await _authRepository.SaveChangesAsync();

            return true;
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var accessTokenMinutes = Convert.ToDouble(_configuration["Jwt:AccessTokenMinutes"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var permissions = await _authRepository.GetUserPermissionsAsync(user.Id);

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private string HashRefreshToken(string refreshToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(bytes);
        }

        private async Task LogLoginActivityAsync(int? userId, string email, string action)
        {
            var collection = _mongoDbService.GetCollection<LoginActivityLog>("login_activity_logs");

            await collection.InsertOneAsync(new LoginActivityLog
            {
                UserId = userId,
                Email = email,
                Action = action,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
                return false;

            user.ResetPasswordToken = new Random().Next(100000, 999999).ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(5);
            user.UpdatedAt = DateTime.UtcNow;

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your password - Flood System",
                $@"
        <h2>Flood System Password Reset</h2>
        <p>Your password reset code is:</p>
        <h1 style='letter-spacing: 8px;'>{user.ResetPasswordToken}</h1>
        <p>This code expires in 5 minutes.</p>
        <p>If you did not request this, you can ignore this email.</p>"
            );

            return true;
        }

        public async Task<bool> VerifyResetCodeAsync(string code)
        {
            var user = await _authRepository.GetUserByResetTokenAsync(code);

            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string code, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                return false;

            var user = await _authRepository.GetUserByResetTokenAsync(code);

            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
                throw new Exception("New password cannot be the same as the current password.");


            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.SaveChangesAsync();

            return true;
        }
    }
}