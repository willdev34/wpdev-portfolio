// ====================================
// Título: LoginResponseDto.cs
// Descrição: DTO para resposta do login com token JWT
// ====================================

using System.Text.Json.Serialization;

namespace Portfolio.Web.DTOs.Auth;

public class LoginResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}