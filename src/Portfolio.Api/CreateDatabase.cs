using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Data;

var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();
optionsBuilder.UseNpgsql("Host=localhost;Database=portfolio_dev;Username=wpdev;Password=Dev@2024!;Port=5432");

using var context = new PortfolioDbContext(optionsBuilder.Options);

Console.WriteLine("Criando banco de dados...");
await context.Database.EnsureCreatedAsync();
Console.WriteLine("✅ Banco de dados criado com sucesso!");