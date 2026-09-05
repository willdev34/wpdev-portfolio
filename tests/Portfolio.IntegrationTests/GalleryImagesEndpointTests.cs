// ====================================
// Título: GalleryImagesEndpointTests.cs
// Descrição: Testes de integração do GalleryImagesController. Inclui
//            teste de regressão para o bug de seguranca onde POST/PUT/DELETE
//            nao tinham nenhuma protecao de autenticacao.
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.GalleryImages;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class GalleryImagesEndpointTests : IntegrationTestBase
{
    public GalleryImagesEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_AllGalleryImages_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/galleryimages");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_CreateGalleryImage_WithoutToken_ReturnsUnauthorized()
    {
        // Teste de regressao: esse controller nao tinha [Authorize] nenhum,
        // entao POST/PUT/DELETE ficavam abertos sem autenticacao. Corrigido
        // durante a Sprint 11. Esse teste garante que nunca mais volte a ficar aberto.
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/galleryimages", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_UpdateGalleryImage_WithoutToken_ReturnsUnauthorized()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/galleryimages", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<GalleryImageDto>();

        // Remove o token pra simular uma chamada nao autenticada
        Client.DefaultRequestHeaders.Authorization = null;

        var updateDto = new UpdateGalleryImageDto
        {
            Id = created!.Id,
            Title = "Tentativa sem token",
            AltText = created.AltText,
            ImageUrl = created.ImageUrl,
            Tags = created.Tags,
            Width = created.Width,
            Height = created.Height,
            FileSizeBytes = created.FileSizeBytes,
            Order = created.Order,
            IsVisible = true
        };

        var response = await Client.PutAsJsonAsync($"/api/galleryimages/{created.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_GalleryImage_WithoutToken_ReturnsUnauthorized()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/galleryimages", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<GalleryImageDto>();

        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.DeleteAsync($"/api/galleryimages/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateGalleryImage_WithToken_ReturnsCreated()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/galleryimages", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<GalleryImageDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be(dto.Title);
    }

    [Fact]
    public async Task Put_UpdateGalleryImage_WithToken_ThenGet_ReflectsChanges()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/galleryimages", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<GalleryImageDto>();

        var updateDto = new UpdateGalleryImageDto
        {
            Id = created!.Id,
            Title = "Título Atualizado via Teste",
            AltText = created.AltText,
            ImageUrl = created.ImageUrl,
            Tags = created.Tags,
            Width = created.Width,
            Height = created.Height,
            FileSizeBytes = created.FileSizeBytes,
            Order = created.Order,
            IsVisible = true
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/galleryimages/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/galleryimages/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<GalleryImageDto>();
        updated!.Title.Should().Be("Título Atualizado via Teste");
    }

    [Fact]
    public async Task Delete_GalleryImage_WithToken_SetsIsVisibleFalse()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/galleryimages", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<GalleryImageDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/galleryimages/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/galleryimages/{created.Id}");
        var afterDelete = await getResponse.Content.ReadFromJsonAsync<GalleryImageDto>();
        afterDelete!.IsVisible.Should().BeFalse();
    }

    private static CreateGalleryImageDto BuildValidCreateDto() => new()
    {
        Title = $"Imagem de Teste {Guid.NewGuid()}",
        AltText = "Texto alternativo gerado pelo teste de integração",
        ImageUrl = "https://res.cloudinary.com/do0uq7w4n/image/upload/teste-galeria.webp",
        Tags = new List<string> { "teste" },
        Width = 1200,
        Height = 800,
        FileSizeBytes = 204800,
        Order = 0,
        IsVisible = true
    };
}