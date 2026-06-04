// ====================================
// Título: LoginRequestDto.cs
// Descrição: DTO para requisição de login
// ====================================

namespace Portfolio.Application.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}