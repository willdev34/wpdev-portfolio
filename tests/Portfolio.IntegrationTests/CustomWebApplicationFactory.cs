// ====================================
// Título: CustomWebApplicationFactory.cs
// Descrição: Sobe a API real (Program.cs) contra um PostgreSQL real,
//            descartavel, via Testcontainers.
// ====================================

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace Portfolio.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("portfolio_integration_tests")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Variaveis de ambiente reais: o Program.cs le a configuracao direto,
        // antes do Build(), entao ConfigureAppConfiguration chegaria tarde demais.
        // Variaveis de ambiente sao lidas logo na criacao do WebApplicationBuilder.
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__Secret", "IntegrationTests-Secret-Key-Not-Used-In-Prod-32Chars");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "wpdev-portfolio-api-tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "wpdev-portfolio-web-tests");
        Environment.SetEnvironmentVariable("Admin__Email", "admin.tests@wpdev.local");
        Environment.SetEnvironmentVariable("Admin__Password", "IntegrationTests@2026!");

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}