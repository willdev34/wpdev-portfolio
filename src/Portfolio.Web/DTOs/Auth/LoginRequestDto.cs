// ====================================
// Título: LoginRequestDto.cs
// Descrição: DTO para requisição de login no admin
// ====================================

namespace Portfolio.Web.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}