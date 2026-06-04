// ====================================
// Título: AuthService.cs
// Descrição: Serviço de autenticação, gerencia login e token JWT
// ====================================

using System.Net.Http.Json;
using System.Text.Json;
using Portfolio.Web.DTOs.Auth;
using Portfolio.Web.Json;

namespace Portfolio.Web.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, AuthJsonContext.Default.LoginRequestDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize(responseJson, AuthJsonContext.Default.LoginResponseDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Erro no login: {ex.Message}");
            return null;
        }
    }
}