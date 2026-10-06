using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Data;

// Subcomando de dados: dotnet run --project tools/DatabaseSetup -- normalize-technologies [--apply]
// Sem argumentos, mantém o comportamento original (criar o banco local).
if (args.Length > 0 && args[0] == "normalize-technologies")
{
    return await DatabaseSetup.TechnologyCleanupRunner.RunAsync(args.Contains("--apply"));
}

Console.WriteLine("🚀 Iniciando criação do banco de dados...");

var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();
optionsBuilder.UseNpgsql("Host=localhost;Database=portfolio_dev;Username=wpdev;Password=Dev@2024!;Port=5432");

using var context = new PortfolioDbContext(optionsBuilder.Options);

Console.WriteLine("📦 Criando tabelas...");
await context.Database.EnsureCreatedAsync();

Console.WriteLine("✅ Banco de dados criado com sucesso!");
Console.WriteLine("✅ Todas as tabelas foram criadas!");

return 0;
