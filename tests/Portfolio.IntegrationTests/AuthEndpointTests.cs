// ====================================
// Título: AuthEndpointTests.cs
// Descrição: Testes de integração do AuthController - login e /me
// ====================================

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.Auth;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class AuthEndpointTests : IntegrationTestBase
{
    public AuthEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var request = new LoginRequestDto { Email = AdminEmail, Password = AdminPassword };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Email.Should().Be(AdminEmail);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = AdminEmail, Password = "SenhaErrada123!" };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "nao-existe@wpdev.local", Password = "QualquerSenha123!" };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WithValidToken_ReturnsOkWithEmail()
    {
        await AuthenticateClientAsync();

        var response = await Client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}