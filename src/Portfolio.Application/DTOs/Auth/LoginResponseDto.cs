// ====================================
// Título: LoginResponseDto.cs
// Descrição: DTO para resposta do login com token JWT
// ====================================

namespace Portfolio.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}