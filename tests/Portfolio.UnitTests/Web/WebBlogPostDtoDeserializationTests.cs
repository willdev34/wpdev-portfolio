// Título: WebBlogPostDtoDeserializationTests.cs
// Descrição: Regressão da tela de edição do admin: o BlogPostDto do Web (linkado no csproj)
//            precisa ler o JSON de um rascunho, que vem com publishedAt nulo da API

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using WebBlogPostDto = Portfolio.Web.DTOs.BlogPosts.BlogPostDto;

namespace Portfolio.UnitTests.Web;

/// <summary>
/// Contexto mínimo com source generator, nas mesmas opções de leitura do BlogPostJsonContext do Web
/// </summary>
[JsonSerializable(typeof(WebBlogPostDto))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class WebBlogPostTestJsonContext : JsonSerializerContext
{
}

public class WebBlogPostDtoDeserializationTests
{
    [Fact]
    public void Deserialize_DeveLerRascunho_QuandoPublishedAtScheduledAtEFeaturedImageUrlNulos()
    {
        // Arrange
        // Formato devolvido por GET api/blogposts/admin/{id} para um rascunho
        const string json = """
        {
          "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
          "title": "Rascunho",
          "slug": "rascunho",
          "excerpt": "Resumo do rascunho.",
          "content": "Conteúdo do rascunho.",
          "featuredImageUrl": null,
          "tags": ["dotnet"],
          "isFeatured": false,
          "isPublished": false,
          "publishedAt": null,
          "readTimeMinutes": 3,
          "viewCount": 0,
          "status": "Draft",
          "scheduledAt": null,
          "createdAt": "2026-10-01T12:00:00Z",
          "updatedAt": null,
          "authorId": null
        }
        """;

        // Act
        var act = () => JsonSerializer.Deserialize(json, WebBlogPostTestJsonContext.Default.BlogPostDto);

        // Assert
        var post = act.Should().NotThrow().Subject;
        post.Should().NotBeNull();
        post!.Status.Should().Be("Draft");
        post.PublishedAt.Should().BeNull();
        post.ScheduledAt.Should().BeNull();
        post.FeaturedImageUrl.Should().BeNull();
        post.Slug.Should().Be("rascunho");
    }

    [Fact]
    public void Deserialize_DeveLerPostPublicado_QuandoTodosOsCamposPreenchidos()
    {
        // Arrange
        const string json = """
        {
          "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3302",
          "title": "Publicado",
          "slug": "publicado",
          "excerpt": "Resumo do publicado.",
          "content": "Conteúdo do publicado.",
          "featuredImageUrl": "https://res.cloudinary.com/demo/image/upload/capa.png",
          "tags": ["dotnet", "blazor"],
          "isFeatured": true,
          "isPublished": true,
          "publishedAt": "2026-10-01T18:00:00Z",
          "readTimeMinutes": 5,
          "viewCount": 42,
          "status": "Published",
          "scheduledAt": "2026-10-01T18:00:00Z",
          "createdAt": "2026-09-30T12:00:00Z",
          "updatedAt": "2026-10-01T18:05:00Z"
        }
        """;

        // Act
        var post = JsonSerializer.Deserialize(json, WebBlogPostTestJsonContext.Default.BlogPostDto);

        // Assert
        post.Should().NotBeNull();
        post!.Status.Should().Be("Published");
        post.PublishedAt.Should().Be(new DateTime(2026, 10, 1, 18, 0, 0, DateTimeKind.Utc));
        post.ScheduledAt.Should().Be(new DateTime(2026, 10, 1, 18, 0, 0, DateTimeKind.Utc));
        post.FeaturedImageUrl.Should().Be("https://res.cloudinary.com/demo/image/upload/capa.png");
        post.Tags.Should().Equal("dotnet", "blazor");
        post.IsFeatured.Should().BeTrue();
        post.ViewCount.Should().Be(42);
    }
}
