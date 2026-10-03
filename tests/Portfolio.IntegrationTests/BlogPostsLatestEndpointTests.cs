// ====================================
// Título: BlogPostsLatestEndpointTests.cs
// Descrição: Testes de integração do GET /api/blogposts/latest (prévia da Home).
//            Cada teste cria os próprios posts (slug único, inseridos pelo DbContext para
//            controlar as datas) e filtra o resultado por eles, sem assertar contagem total.
//
//            Como /latest devolve só os N mais recentes do banco inteiro, os posts de cada
//            teste ficam numa "era" de datas no futuro (EraBase), crescente a cada teste:
//            sempre acima dos posts de outros testes, então ocupam o topo de forma estável.
// ====================================

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class BlogPostsLatestEndpointTests : IntegrationTestBase
{
    private readonly CustomWebApplicationFactory _factory;

    // Cada teste ganha uma era 10 dias acima da anterior (os offsets dentro do teste são em horas)
    private static int _eraCounter;

    public BlogPostsLatestEndpointTests(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Latest_Anonymous_ReturnsOk()
    {
        var response = await GetLatestAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Latest_RouteIsNotCapturedByIdRoute_ReturnsListNotPostNotFound()
    {
        // "latest" não é um Guid: se caísse em /{id}, o resultado seria 404/400 de post, não uma lista
        var response = await GetLatestAsync();
        var json = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        json.Should().StartWith("[");
    }

    [Fact]
    public async Task Get_Latest_ExcludesDraftAndFutureScheduled()
    {
        var era = NewEra();
        var unique = NewUnique();
        var published = BuildPost($"latest-published-{unique}", BlogPostStatus.Published, publishedAt: era);
        // Se a regra pública falhasse, estes dois ficariam ACIMA do publicado
        var draft = BuildPost($"latest-draft-{unique}", BlogPostStatus.Draft, createdAt: era.AddHours(2));
        var future = BuildPost($"latest-future-{unique}", BlogPostStatus.Scheduled,
            scheduledAt: DateTime.UtcNow.AddDays(5), publishedAt: era.AddHours(3));
        await SeedAsync(published, draft, future);

        var posts = await GetLatestPostsAsync(12);
        var slugs = OursOf(posts, unique);

        slugs.Should().Contain(published.Slug);
        slugs.Should().NotContain(draft.Slug);
        slugs.Should().NotContain(future.Slug);
    }

    [Fact]
    public async Task Get_Latest_IncludesOverdueScheduled()
    {
        var era = NewEra();
        var unique = NewUnique();
        // Agendado cuja data já venceu. PublishedAt na era só para entrar no topo do ranking
        var overdue = BuildPost($"latest-overdue-{unique}", BlogPostStatus.Scheduled,
            scheduledAt: DateTime.UtcNow.AddDays(-2), publishedAt: era);
        await SeedAsync(overdue);

        var posts = await GetLatestPostsAsync(12);

        OursOf(posts, unique).Should().Contain(overdue.Slug);
    }

    [Fact]
    public async Task Get_Latest_RespectsCount()
    {
        var era = NewEra();
        var unique = NewUnique();
        var newest = BuildPost($"latest-count-newest-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(3));
        var middle = BuildPost($"latest-count-middle-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(2));
        var oldest = BuildPost($"latest-count-oldest-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(1));
        await SeedAsync(oldest, newest, middle);

        var one = await GetLatestPostsAsync(1);
        var two = await GetLatestPostsAsync(2);

        // O tamanho da resposta é limitado pelo count; os posts do topo são os deste teste
        one.Select(p => p.Slug).Should().Equal(newest.Slug);
        two.Select(p => p.Slug).Should().Equal(newest.Slug, middle.Slug);
    }

    [Fact]
    public async Task Get_Latest_WithoutCount_UsesDefaultOfThree()
    {
        var era = NewEra();
        var unique = NewUnique();
        var posts = Enumerable.Range(1, 4)
            .Select(i => BuildPost($"latest-default-{i}-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(i)))
            .ToArray();
        await SeedAsync(posts);

        var response = await GetLatestAsync(count: null);
        var result = await response.Content.ReadFromJsonAsync<List<BlogPostCardDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCount(3);
        result!.Select(p => p.Slug).Should().OnlyContain(s => s.EndsWith(unique));
    }

    [Fact]
    public async Task Get_Latest_OrdersMostRecentFirst()
    {
        var era = NewEra();
        var unique = NewUnique();
        var newest = BuildPost($"latest-order-newest-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(3));
        var middle = BuildPost($"latest-order-middle-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(2));
        var oldest = BuildPost($"latest-order-oldest-{unique}", BlogPostStatus.Published, publishedAt: era.AddHours(1));
        // Insere fora de ordem para não depender da ordem de inserção
        await SeedAsync(middle, oldest, newest);

        var posts = await GetLatestPostsAsync(3);

        posts.Select(p => p.Slug).Should().Equal(newest.Slug, middle.Slug, oldest.Slug);
    }

    [Fact]
    public async Task Get_Latest_PostWithNullPublishedAt_IsRankedByCreatedAt()
    {
        // Documenta a ordenação (PublishedAt ?? CreatedAt) desc: sem PublishedAt, vale o CreatedAt
        var era = NewEra();
        var unique = NewUnique();
        // CreatedAt mais recente que os outros dois, mas sem PublishedAt: deve vir primeiro
        var noPublishedAt = BuildPost($"latest-null-{unique}", BlogPostStatus.Published,
            publishedAt: null, createdAt: era.AddHours(3));
        var withPublishedAt = BuildPost($"latest-withdate-{unique}", BlogPostStatus.Published,
            publishedAt: era.AddHours(2), createdAt: era.AddDays(-30));
        // PublishedAt nulo e CreatedAt antigo: fica por último
        var noPublishedAtOld = BuildPost($"latest-null-old-{unique}", BlogPostStatus.Published,
            publishedAt: null, createdAt: era.AddHours(1));
        await SeedAsync(withPublishedAt, noPublishedAtOld, noPublishedAt);

        var posts = await GetLatestPostsAsync(3);

        posts.Select(p => p.Slug).Should().Equal(noPublishedAt.Slug, withPublishedAt.Slug, noPublishedAtOld.Slug);
        posts.First(p => p.Slug == noPublishedAt.Slug).PublishedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public async Task Get_Latest_WithInvalidCount_ReturnsBadRequest(int count)
    {
        var response = await GetLatestAsync(count);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Latest_JsonDoesNotExposeStatusNorScheduledAt()
    {
        var era = NewEra();
        var unique = NewUnique();
        await SeedAsync(BuildPost($"latest-json-{unique}", BlogPostStatus.Published, publishedAt: era));

        var response = await GetLatestAsync(1);
        var json = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json.Should().StartWith("[").And.NotBe("[]");
        json.Should().NotContainEquivalentOf("\"status\"");
        json.Should().NotContainEquivalentOf("\"scheduledAt\"");
    }

    [Fact]
    public async Task Get_Latest_ResponseIsCacheableForSixtySeconds()
    {
        var response = await GetLatestAsync();

        response.Headers.CacheControl.Should().NotBeNull();
        response.Headers.CacheControl!.Public.Should().BeTrue();
        response.Headers.CacheControl.MaxAge.Should().Be(TimeSpan.FromSeconds(60));
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private async Task<HttpResponseMessage> GetLatestAsync(int? count = 3)
    {
        var url = count.HasValue ? $"/api/blogposts/latest?count={count}" : "/api/blogposts/latest";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        // O endpoint tem ResponseCache de 60s e a API usa UseResponseCaching: sem isto, um teste
        // receberia a resposta em cache de outro teste que pediu a mesma URL
        request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        return await Client.SendAsync(request);
    }

    private async Task<List<BlogPostCardDto>> GetLatestPostsAsync(int count)
    {
        var response = await GetLatestAsync(count);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<BlogPostCardDto>>();
        return posts!;
    }

    private static List<string> OursOf(List<BlogPostCardDto> posts, string unique) =>
        posts.Select(p => p.Slug).Where(s => s.EndsWith(unique)).ToList();

    private static DateTime NewEra() =>
        DateTime.UtcNow.AddYears(5).AddDays(Interlocked.Increment(ref _eraCounter) * 10);

    private async Task SeedAsync(params BlogPost[] posts)
    {
        // Insere direto pelo DbContext: o AddAsync do repositório sobrescreve CreatedAt
        // e a API não permite PublishedAt nulo num post publicado
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
        Excerpt = "Resumo gerado pelo teste de últimos posts.",
        Content = "Conteúdo gerado pelo teste de integração dos últimos posts.",
        Tags = new List<string> { "teste" },
        ReadTimeMinutes = 3,
        Status = status,
        IsPublished = status == BlogPostStatus.Published,
        ScheduledAt = scheduledAt,
        PublishedAt = publishedAt,
        CreatedAt = createdAt ?? DateTime.UtcNow
    };
}
