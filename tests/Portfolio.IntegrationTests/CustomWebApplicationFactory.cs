// ====================================
// Título: CustomWebApplicationFactory.cs
// Descrição: Sobe a API real (Program.cs) contra um PostgreSQL real,
//            descartavel, via Testcontainers. Cada classe de teste que
//            usar essa factory compartilha o mesmo container e banco.
// ====================================

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString(),
                ["Jwt:Secret"] = "IntegrationTests-Secret-Key-Not-Used-In-Prod-32Chars",
                ["Jwt:Issuer"] = "wpdev-portfolio-api-tests",
                ["Jwt:Audience"] = "wpdev-portfolio-web-tests",
                ["Admin:Email"] = "admin.tests@wpdev.local",
                ["Admin:Password"] = "IntegrationTests@2026!"
            };

            config.AddInMemoryCollection(testConfig);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}