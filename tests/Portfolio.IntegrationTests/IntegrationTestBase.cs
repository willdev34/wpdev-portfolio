// ====================================
// Título: IntegrationTestBase.cs
// Descrição: Classe base que compartilha a mesma factory/container
//            entre todos os testes de uma mesma classe, e centraliza
//            a autenticacao pra testes de rotas protegidas
// ====================================

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.Auth;

namespace Portfolio.IntegrationTests;

public abstract class IntegrationTestBase
{
    // Credenciais definidas em CustomWebApplicationFactory (Admin__Email / Admin__Password)
    protected const string AdminEmail = "admin.tests@wpdev.local";
    protected const string AdminPassword = "IntegrationTests@2026!";

    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Faz login com o admin de teste e retorna o token JWT puro,
    /// para testes que precisam inspecionar o token em si.
    /// </summary>
    protected async Task<string> LoginAndGetTokenAsync()
    {
        var request = new LoginRequestDto { Email = AdminEmail, Password = AdminPassword };
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);
        var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return body!.Token;
    }

    /// <summary>
    /// Autentica o HttpClient da instancia atual, adicionando o header
    /// Authorization em todas as chamadas seguintes feitas por essa classe de teste.
    /// </summary>
    protected async Task AuthenticateClientAsync()
    {
        var token = await LoginAndGetTokenAsync();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}