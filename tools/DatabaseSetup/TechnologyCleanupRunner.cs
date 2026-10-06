// Título: TechnologyCleanupRunner.cs
// Descrição: Comando "normalize-technologies": dry-run por padrão, grava só com --apply, em transação

using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Data;

namespace DatabaseSetup;

public static class TechnologyCleanupRunner
{
    /// <summary>Única fonte da connection string. Nunca vem de arquivo versionado e nunca é escrita no console.</summary>
    public const string ConnectionStringVariable = "PORTFOLIO_CONNECTION_STRING";

    /// <summary>
    /// Retorna 0 em sucesso, 2 se a variável de ambiente faltar e 1 se a verificação pós-gravação falhar.
    /// Não altera UpdatedAt: só a coluna Technologies dos projetos que mudam.
    /// </summary>
    public static async Task<int> RunAsync(bool apply)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.Error.WriteLine($"Defina a variável de ambiente {ConnectionStringVariable} com a connection string do banco.");
            return 2;
        }

        var options = new DbContextOptionsBuilder<PortfolioDbContext>().UseNpgsql(connectionString).Options;
        await using var context = new PortfolioDbContext(options);

        var projects = await context.Projects.ToListAsync();
        var changes = TechnologyCleanupPlanner.Plan(
            projects.Select(p => new ProjectTechnologies(p.Id, p.Title, p.Technologies.ToList())));

        Console.WriteLine($"Projetos lidos: {projects.Count}. Projetos a corrigir: {changes.Count}.");
        foreach (var change in changes)
        {
            Console.WriteLine($"- {change.Title}");
            Console.WriteLine($"    antes:  {string.Join(", ", change.Before)}");
            Console.WriteLine($"    depois: {string.Join(", ", change.After)}");
        }

        if (!apply)
        {
            Console.WriteLine("Dry-run: nada foi gravado. Rode de novo com --apply para gravar.");
            return 0;
        }

        if (changes.Count == 0)
        {
            Console.WriteLine("Nada a gravar: as tecnologias já estão normalizadas.");
            return 0;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();

        var byId = projects.ToDictionary(p => p.Id);
        foreach (var change in changes)
        {
            byId[change.Id].Technologies = change.After.ToList();
        }

        var written = await context.SaveChangesAsync();

        // Confere dentro da transação: reler do banco e planejar de novo precisa dar zero mudanças
        context.ChangeTracker.Clear();
        var reread = await context.Projects.AsNoTracking().ToListAsync();
        var remaining = TechnologyCleanupPlanner.Plan(
            reread.Select(p => new ProjectTechnologies(p.Id, p.Title, p.Technologies.ToList())));

        if (remaining.Count > 0)
        {
            await transaction.RollbackAsync();
            Console.Error.WriteLine($"Verificação falhou ({remaining.Count} projetos ainda mudariam). Transação desfeita, nada foi gravado.");
            return 1;
        }

        await transaction.CommitAsync();
        Console.WriteLine($"Gravado: {written} projetos atualizados.");
        return 0;
    }
}
