// ====================================
// Título: BlogPostsVisibilityEndpointTests.cs
// Descrição: Testes de integração da regra pública de BlogPosts nos endpoints
//            (Draft, Scheduled e Published), autenticação do admin e contrato do JSON público
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Domain.Enums;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class BlogPostsVisibilityEndpointTests : IntegrationTestBase
{
    public BlogPostsVisibilityEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_PublicBlogPosts_ExcludesDraftAndFutureScheduled()
    {
        await AuthenticateClientAsync();
        var published = await CreatePostAsync(BlogPostStatus.Published);
        var draft = await CreatePostAsync(BlogPostStatus.Draft);
        var future = await CreatePostAsync(BlogPostStatus.Scheduled, DateTime.UtcNow.AddDays(5));
        ClearAuthentication();

        var response = await Client.GetAsync("/api/blogposts/public");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<BlogPostCardDto>>();
        var slugs = posts!.Select(p => p.Slug).ToList();
        slugs.Should().Contain(published.Slug);
        slugs.Should().NotContain(draft.Slug);
        slugs.Should().NotContain(future.Slug);
    }

    [Fact]
    public async Task Get_PublicBlogPosts_JsonDoesNotExposeStatusNorScheduledAt()
    {
        await AuthenticateClientAsync();
        await CreatePostAsync(BlogPostStatus.Published);
        ClearAuthentication();

        var response = await Client.GetAsync("/api/blogposts/public");
        var json = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json.Should().StartWith("[").And.NotBe("[]");
        json.Should().NotContainEquivalentOf("\"status\"");
        json.Should().NotContainEquivalentOf("\"scheduledAt\"");
    }

    [Fact]
    public async Task Get_AllBlogPosts_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.GetAsync("/api/blogposts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_AllBlogPosts_WithToken_ReturnsEveryStatusWithStatusAndScheduledAt()
    {
        await AuthenticateClientAsync();
        var scheduledAt = DateTime.UtcNow.AddDays(5);
        var published = await CreatePostAsync(BlogPostStatus.Published);
        var draft = await CreatePostAsync(BlogPostStatus.Draft);
        var future = await CreatePostAsync(BlogPostStatus.Scheduled, scheduledAt);

        var response = await Client.GetAsync("/api/blogposts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<BlogPostAdminCardDto>>();
        var ours = posts!.Where(p => p.Id == published.Id || p.Id == draft.Id || p.Id == future.Id)
            .ToDictionary(p => p.Id);

        ours.Should().HaveCount(3);
        ours[published.Id].Status.Should().Be("Published");
        ours[published.Id].ScheduledAt.Should().BeNull();
        ours[draft.Id].Status.Should().Be("Draft");
        ours[draft.Id].ScheduledAt.Should().BeNull();
        ours[future.Id].Status.Should().Be("Scheduled");
        ours[future.Id].ScheduledAt.Should().NotBeNull();
        ours[future.Id].ScheduledAt!.Value.ToUniversalTime().Should().BeCloseTo(scheduledAt, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Get_BlogPostBySlug_Draft_Anonymous_ReturnsNotFound()
    {
        await AuthenticateClientAsync();
        var draft = await CreatePostAsync(BlogPostStatus.Draft);
        ClearAuthentication();

        var response = await Client.GetAsync($"/api/blogposts/slug/{draft.Slug}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_BlogPostById_Draft_Anonymous_ReturnsNotFound()
    {
        await AuthenticateClientAsync();
        var draft = await CreatePostAsync(BlogPostStatus.Draft);
        ClearAuthentication();

        var response = await Client.GetAsync($"/api/blogposts/{draft.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_BlogPostByIdAdmin_WithoutToken_ReturnsUnauthorized()
    {
        await AuthenticateClientAsync();
        var draft = await CreatePostAsync(BlogPostStatus.Draft);
        ClearAuthentication();

        var response = await Client.GetAsync($"/api/blogposts/admin/{draft.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_BlogPostByIdAdmin_Draft_WithToken_ReturnsOk()
    {
        await AuthenticateClientAsync();
        var draft = await CreatePostAsync(BlogPostStatus.Draft);

        var response = await Client.GetAsync($"/api/blogposts/admin/{draft.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        body!.Id.Should().Be(draft.Id);
        body.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task Post_CreateBlogPost_ScheduledInThePast_ReturnsBadRequest()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();
        dto.Status = BlogPostStatus.Scheduled;
        dto.ScheduledAt = DateTime.UtcNow.AddDays(-1);

        var response = await Client.PostAsJsonAsync("/api/blogposts", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<BlogPostDto> CreatePostAsync(BlogPostStatus status, DateTime? scheduledAt = null)
    {
        var dto = BuildValidCreateDto();
        dto.Status = status;
        dto.ScheduledAt = scheduledAt;
        dto.IsPublished = status == BlogPostStatus.Published;

        var response = await Client.PostAsJsonAsync("/api/blogposts", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        return created!;
    }

    private void ClearAuthentication()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    private static CreateBlogPostDto BuildValidCreateDto()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        return new CreateBlogPostDto
        {
            Title = $"Post de Visibilidade {unique}",
            Slug = $"post-de-visibilidade-{unique}",
            Excerpt = "Resumo gerado pelo teste de integração.",
            Content = "Conteúdo completo gerado pelo teste de integração, em Markdown.",
            Tags = new List<string> { "teste", "integracao" },
            IsFeatured = false,
            IsPublished = true,
            ReadTimeMinutes = 3
        };
    }
}
