// ====================================
// Título: NowSectionsEndpointTests.cs
// Descrição: Testes de integração do NowSectionsController - rota
//            pública /api/nowsection (sem 's'), CRUD admin, e a regra
//            de negócio de que só uma seção pode estar ativa por vez.
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class NowSectionsEndpointTests : IntegrationTestBase
{
    public NowSectionsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_ActiveNowSection_Public_DoesNotRequireToken()
    {
        // Rota /api/nowsection (sem 's') é a única publica desse controller
        var response = await Client.GetAsync("/api/nowsection");

        // Pode ser 200 (se ja existir uma ativa de outro teste) ou 404
        // (se nenhuma existir ainda), mas nunca 401.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_AllNowSections_WithoutToken_ReturnsUnauthorized()
    {
        // Diferente de Projects/BlogPosts/Timeline: aqui o GET com 's' (admin)
        // nao tem [AllowAnonymous], entao exige token.
        var response = await Client.GetAsync("/api/nowsections");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateNowSection_WithoutToken_ReturnsUnauthorized()
    {
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/nowsections", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateNowSection_WithToken_ReturnsCreated_AndIsActive()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/nowsections", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<NowSectionDto>();
        created.Should().NotBeNull();
        created!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Post_CreateSecondNowSection_DeactivatesThePrevious()
    {
        // Regra de negocio central desse controller: só 1 secao ativa por vez.
        await AuthenticateClientAsync();

        var firstDto = BuildValidCreateDto();
        var firstResponse = await Client.PostAsJsonAsync("/api/nowsections", firstDto);
        var first = await firstResponse.Content.ReadFromJsonAsync<NowSectionDto>();

        var secondDto = BuildValidCreateDto();
        var secondResponse = await Client.PostAsJsonAsync("/api/nowsections", secondDto);
        var second = await secondResponse.Content.ReadFromJsonAsync<NowSectionDto>();

        second!.IsActive.Should().BeTrue();

        var firstAfter = await Client.GetAsync($"/api/nowsections/{first!.Id}");
        var firstAfterBody = await firstAfter.Content.ReadFromJsonAsync<NowSectionDto>();
        firstAfterBody!.IsActive.Should().BeFalse();

        // E a rota publica agora deve refletir a segunda seção como a ativa
        var activeResponse = await Client.GetAsync("/api/nowsection");
        var active = await activeResponse.Content.ReadFromJsonAsync<NowSectionDto>();
        active!.Id.Should().Be(second.Id);
    }

    [Fact]
    public async Task Put_UpdateNowSection_ThenGet_ReflectsChanges()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/nowsections", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<NowSectionDto>();

        var updateDto = new UpdateNowSectionDto
        {
            Id = created!.Id,
            Content = "Conteúdo atualizado via teste de integração, com bastante detalhe.",
            CurrentProjects = created.CurrentProjects,
            CurrentlyLearning = created.CurrentlyLearning,
            CurrentGoals = created.CurrentGoals,
            IsActive = true
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/nowsections/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/nowsections/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<NowSectionDto>();
        updated!.Content.Should().Be(updateDto.Content);
    }

    [Fact]
    public async Task Delete_NowSection_SetsIsActiveFalse()
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/nowsections", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<NowSectionDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/nowsections/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/nowsections/{created.Id}");
        var afterDelete = await getResponse.Content.ReadFromJsonAsync<NowSectionDto>();
        afterDelete!.IsActive.Should().BeFalse();
    }

    private static CreateNowSectionDto BuildValidCreateDto() => new()
    {
        Content = "Trabalhando no WPDev Portfolio, gerado pelo teste de integração.",
        CurrentProjects = new List<ProjectLink>
        {
            new() { Name = "WPDev Portfolio", Url = "https://wpdev-portfolio-web.onrender.com" }
        },
        CurrentlyLearning = new List<string> { "Testes de integração" },
        CurrentGoals = new List<string> { "Cobrir 100% dos controllers admin" }
    };
}