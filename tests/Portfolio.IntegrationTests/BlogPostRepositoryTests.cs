// ====================================
// Título: BlogPostRepositoryTests.cs
// Descrição: Testes de integração do BlogPostRepository.GetPublishedAsync
//            contra o PostgreSQL do Testcontainers (regra pública e ordenação)
// ====================================

using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class BlogPostRepositoryTests : IntegrationTestBase
{
    // Banco compartilhado entre testes: cada teste cria posts com slug único
    // e filtra o resultado por esses slugs, sem assertar contagem total nem posição global
    private readonly CustomWebApplicationFactory _factory;

    public BlogPostRepositoryTests(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPublishedAsync_ReturnsPublishedAndOverdueScheduled_ExcludesDraftAndFutureScheduled()
    {
        var unique = NewUnique();
        var published = BuildPost($"repo-published-{unique}", BlogPostStatus.Published, publishedAt: DateTime.UtcNow.AddDays(-3));
        var overdue = BuildPost($"repo-overdue-{unique}", BlogPostStatus.Scheduled, scheduledAt: DateTime.UtcNow.AddDays(-2));
        var draft = BuildPost($"repo-draft-{unique}", BlogPostStatus.Draft);
        var future = BuildPost($"repo-future-{unique}", BlogPostStatus.Scheduled, scheduledAt: DateTime.UtcNow.AddDays(5));
        await SeedAsync(published, overdue, draft, future);

        var ours = await GetPublishedSlugsAsync(unique);

        ours.Should().Contain(published.Slug);
        ours.Should().Contain(overdue.Slug);
        ours.Should().NotContain(draft.Slug);
        ours.Should().NotContain(future.Slug);
    }

    [Fact]
    public async Task GetPublishedAsync_OrdersByPublishedAtDesc_ThenByCreatedAtDesc()
    {
        var unique = NewUnique();
        var samePublishedAt = DateTime.UtcNow.AddDays(-5);

        var newest = BuildPost($"order-newest-{unique}", BlogPostStatus.Published,
            publishedAt: DateTime.UtcNow.AddDays(-1), createdAt: DateTime.UtcNow.AddDays(-10));
        // Mesmo PublishedAt: desempata por CreatedAt decrescente
        var tieCreatedLater = BuildPost($"order-tie-later-{unique}", BlogPostStatus.Published,
            publishedAt: samePublishedAt, createdAt: DateTime.UtcNow.AddDays(-6));
        var tieCreatedEarlier = BuildPost($"order-tie-earlier-{unique}", BlogPostStatus.Published,
            publishedAt: samePublishedAt, createdAt: DateTime.UtcNow.AddDays(-8));
        var oldest = BuildPost($"order-oldest-{unique}", BlogPostStatus.Published,
            publishedAt: DateTime.UtcNow.AddDays(-9), createdAt: DateTime.UtcNow.AddDays(-1));

        // Insere fora de ordem para não depender da ordem de inserção
        await SeedAsync(oldest, tieCreatedEarlier, newest, tieCreatedLater);

        var ours = await GetPublishedSlugsAsync(unique);

        // Ordem relativa apenas entre os posts deste teste
        ours.Should().Equal(newest.Slug, tieCreatedLater.Slug, tieCreatedEarlier.Slug, oldest.Slug);
    }

    private async Task<List<string>> GetPublishedSlugsAsync(string unique)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var repository = new BlogPostRepository(context);

        var result = await repository.GetPublishedAsync();

        return result.Select(p => p.Slug).Where(s => s.EndsWith(unique)).ToList();
    }

    private async Task SeedAsync(params BlogPost[] posts)
    {
        // Insere direto pelo DbContext: o AddAsync do repositório sobrescreve CreatedAt,
        // e o teste de ordenação precisa controlar o desempate
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        context.BlogPosts.AddRange(posts);
        await context.SaveChangesAsync();
    }

    private static string NewUnique() => Guid.NewGuid().ToString("N")[..8];

    private static BlogPost BuildPost(
        string slug,
        BlogPostStatus status,
        DateTime? publishedAt = null,
        DateTime? scheduledAt = null,
        DateTime? createdAt = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = $"Post {slug}",
        Slug = slug,
        Excerpt = "Resumo gerado pelo teste de repositório.",
        Content = "Conteúdo gerado pelo teste de integração do repositório de posts.",
        Tags = new List<string> { "teste" },
        ReadTimeMinutes = 3,
        Status = status,
        IsPublished = status == BlogPostStatus.Published,
        ScheduledAt = scheduledAt,
        // Scheduled grava PublishedAt igual ao ScheduledAt, como o handler faz
        PublishedAt = publishedAt ?? scheduledAt,
        CreatedAt = createdAt ?? DateTime.UtcNow
    };
}
