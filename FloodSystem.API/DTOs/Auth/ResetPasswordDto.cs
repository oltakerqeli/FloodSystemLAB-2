namespace FloodSystem.API.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Code { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}