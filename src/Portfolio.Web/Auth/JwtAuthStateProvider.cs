// ====================================
// Título: JwtAuthStateProvider.cs
// Descrição: Provider de autenticação baseado em JWT para Blazor WASM
// ====================================

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Portfolio.Web.Auth;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;
    private const string TokenKey = "wpdev_auth_token";

    public JwtAuthStateProvider(IJSRuntime js, HttpClient http)
    {
        _js = js;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            if (string.IsNullOrWhiteSpace(token))
                return Anonymous();

            // Verifica se o token expirou
            if (IsTokenExpired(token))
            {
                await RemoveTokenAsync();
                return Anonymous();
            }

            // Adiciona o token no header do HttpClient
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return Anonymous();
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var payload = ParsePayload(token);
            if (payload.TryGetValue("exp", out var exp))
            {
                var expValue = ((JsonElement)exp).GetInt64();
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expValue);
                return expDate < DateTimeOffset.UtcNow;
            }
            return true;
        }
        catch
        {
            return true;
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var claims = new List<Claim>();
        var payload = ParsePayload(token);

        foreach (var kvp in payload)
        {
            var value = kvp.Value is JsonElement element
                ? element.ToString()
                : kvp.Value?.ToString() ?? string.Empty;

            claims.Add(new Claim(kvp.Key, value));
        }

        return claims;
    }

    private static Dictionary<string, object> ParsePayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return new();

        var payload = parts[1];

        // Corrige padding base64
        payload = payload.Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var bytes = Convert.FromBase64String(payload);
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<Dictionary<string, object>>(json)
               ?? new();
    }
}