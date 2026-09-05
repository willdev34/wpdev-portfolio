// ====================================
// Título: BlogPostsEndpointTests.cs
// Descrição: Testes de integração do BlogPostsController - CRUD,
//            slug duplicado, e incremento de ViewCount via GetBySlug
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.BlogPosts;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class BlogPostsEndpointTests : IntegrationTestBase
{
    public BlogPostsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_AllBlogPosts_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/blogposts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_CreateBlogPost_WithoutToken_ReturnsUnauthorized()
    {
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/blogposts", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateBlogPost_WithToken_ReturnsCreated()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/blogposts", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<BlogPostDto>();
        created.Should().NotBeNull();
        created!.Slug.Should().Be(dto.Slug);
    }

    [Fact]
    public async Task Post_CreateBlogPost_WithDuplicateSlug_ReturnsBadRequest()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var firstResponse = await Client.PostAsJsonAsync("/api/blogposts", dto);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Mesmo slug, titulo diferente
        var duplicateDto = BuildValidCreateDto();
        duplicateDto.Slug = dto.Slug;

        var secondResponse = await Client.PostAsJsonAsync("/api/blogposts", duplicateDto);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_BlogPostBySlug_IncrementsViewCount()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/blogposts", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<BlogPostDto>();
        created!.ViewCount.Should().Be(0);

        var firstView = await Client.GetAsync($"/api/blogposts/slug/{dto.Slug}");
        var firstBody = await firstView.Content.ReadFromJsonAsync<BlogPostDto>();

        var secondView = await Client.GetAsync($"/api/blogposts/slug/{dto.Slug}");
        var secondBody = await secondView.Content.ReadFromJsonAsync<BlogPostDto>();

        firstBody!.ViewCount.Should().Be(1);
        secondBody!.ViewCount.Should().Be(2);
    }

    [Fact]
    public async Task Put_UpdateBlogPost_ThenGet_ReflectsChanges()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/blogposts", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<BlogPostDto>();

        var updateDto = new UpdateBlogPostDto
        {
            Id = created!.Id,
            Title = "Título Atualizado via Teste",
            Slug = created.Slug,
            Excerpt = createDto.Excerpt,
            Content = createDto.Content,
            Tags = createDto.Tags,
            IsFeatured = createDto.IsFeatured,
            IsPublished = createDto.IsPublished,
            ReadTimeMinutes = createDto.ReadTimeMinutes
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/blogposts/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/blogposts/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<BlogPostDto>();
        updated!.Title.Should().Be("Título Atualizado via Teste");
    }

    [Fact]
    public async Task Delete_BlogPost_ThenGet_ReturnsNotFound()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/blogposts", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<BlogPostDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/blogposts/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/blogposts/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static CreateBlogPostDto BuildValidCreateDto()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        return new CreateBlogPostDto
        {
            Title = $"Post de Teste {unique}",
            Slug = $"post-de-teste-{unique}",
            Excerpt = "Resumo gerado pelo teste de integração.",
            Content = "Conteúdo completo gerado pelo teste de integração, em Markdown.",
            Tags = new List<string> { "teste", "integracao" },
            IsFeatured = false,
            IsPublished = true,
            ReadTimeMinutes = 3
        };
    }
}