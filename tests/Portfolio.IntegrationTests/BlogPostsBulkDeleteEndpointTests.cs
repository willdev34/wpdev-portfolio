// ====================================
// Título: BlogPostsBulkDeleteEndpointTests.cs
// Descrição: Testes de integração do POST /api/blogposts/bulk-delete.
//            Cada teste cria os próprios posts (slug único) e envia só esses ids,
//            porque o banco é compartilhado
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class BlogPostsBulkDeleteEndpointTests : IntegrationTestBase
{
    private const string Url = "/api/blogposts/bulk-delete";

    public BlogPostsBulkDeleteEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Post_BulkDelete_WithoutToken_ReturnsUnauthorized()
    {
        var dto = new BulkDeleteBlogPostsDto { Ids = new List<Guid> { Guid.NewGuid() } };

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_BulkDelete_WithEmptyList_ReturnsBadRequest()
    {
        await AuthenticateClientAsync();

        var response = await Client.PostAsJsonAsync(Url, new BulkDeleteBlogPostsDto());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_BulkDelete_WithNonexistentIds_ReturnsOkWithZero()
    {
        await AuthenticateClientAsync();
        var dto = new BulkDeleteBlogPostsDto { Ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() } };

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BulkDeleteBlogPostsResultDto>();
        result!.DeletedCount.Should().Be(0);
    }

    [Fact]
    public async Task Post_BulkDelete_WithExistingPosts_DeletesThemAndReturnsCount()
    {
        await AuthenticateClientAsync();
        var first = await CreatePostAsync();
        var second = await CreatePostAsync();
        var dto = new BulkDeleteBlogPostsDto { Ids = new List<Guid> { first, second } };

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BulkDeleteBlogPostsResultDto>();
        result!.DeletedCount.Should().Be(2);

        (await Client.GetAsync($"/api/blogposts/{first}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Client.GetAsync($"/api/blogposts/{second}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_BulkDelete_WithExistingAndNonexistentIds_CountsOnlyExisting()
    {
        await AuthenticateClientAsync();
        var existing = await CreatePostAsync();
        var dto = new BulkDeleteBlogPostsDto { Ids = new List<Guid> { existing, Guid.NewGuid() } };

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BulkDeleteBlogPostsResultDto>();
        result!.DeletedCount.Should().Be(1);
    }

    [Fact]
    public async Task Post_BulkDelete_WithDuplicatedIds_CountsOnce()
    {
        await AuthenticateClientAsync();
        var id = await CreatePostAsync();
        var dto = new BulkDeleteBlogPostsDto { Ids = new List<Guid> { id, id, id } };

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BulkDeleteBlogPostsResultDto>();
        result!.DeletedCount.Should().Be(1);
    }

    /// <summary>
    /// Cria um post com slug único e devolve o id. É o único tipo de id enviado à exclusão.
    /// </summary>
    private async Task<Guid> CreatePostAsync()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var dto = new CreateBlogPostDto
        {
            Title = $"Post Bulk Teste {unique}",
            Slug = $"post-bulk-teste-{unique}",
            Excerpt = "Resumo gerado pelo teste de exclusão em massa.",
            Content = "Conteúdo completo gerado pelo teste de exclusão em massa, em Markdown.",
            Tags = new List<string> { "teste" },
            IsFeatured = false,
            IsPublished = true,
            ReadTimeMinutes = 3
        };

        var response = await Client.PostAsJsonAsync("/api/blogposts", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        return created!.Id;
    }
}
