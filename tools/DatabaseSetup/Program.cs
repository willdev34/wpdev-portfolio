using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Data;

Console.WriteLine("🚀 Iniciando criação do banco de dados...");

var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();
optionsBuilder.UseNpgsql("Host=localhost;Database=portfolio_dev;Username=wpdev;Password=Dev@2024!;Port=5432");

using var context = new PortfolioDbContext(optionsBuilder.Options);

Console.WriteLine("📦 Criando tabelas...");
await context.Database.EnsureCreatedAsync();

Console.WriteLine("✅ Banco de dados criado com sucesso!");
Console.WriteLine("✅ Todas as tabelas foram criadas!");