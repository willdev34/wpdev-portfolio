using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class ProjectsEndpointTests : IntegrationTestBase
{
    public ProjectsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_AllProjects_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/projects");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectCardDto>>();
        projects.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ProjectById_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var response = await Client.GetAsync($"/api/projects/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CreateProject_WithoutToken_ReturnsUnauthorized()
    {
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/projects", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateProject_WithToken_ReturnsCreated()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/projects", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<ProjectDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be(dto.Title);
    }

    [Fact]
    public async Task Put_UpdateProject_ThenGet_ReflectsChanges()
    {
        await AuthenticateClientAsync();

        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/projects", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        var updateDto = new UpdateProjectDto
        {
            Id = created!.Id,
            Title = "Título Atualizado via Teste",
            Description = createDto.Description,
            ImageUrl = createDto.ImageUrl,
            Technologies = createDto.Technologies,
            Year = createDto.Year,
            IsFeatured = createDto.IsFeatured,
            Status = createDto.Status,
            IsActive = true
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/projects/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/projects/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        updated!.Title.Should().Be("Título Atualizado via Teste");
    }

    [Fact]
    public async Task Delete_Project_ThenGet_ReturnsNotFound()
    {
        await AuthenticateClientAsync();

        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/projects", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/projects/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/projects/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static CreateProjectDto BuildValidCreateDto() => new()
    {
        Title = $"Projeto de Teste {Guid.NewGuid()}",
        Description = "Descrição gerada pelo teste de integração, com mais de dez caracteres.",
        ImageUrl = "https://res.cloudinary.com/do0uq7w4n/image/upload/teste.webp",
        Technologies = new List<string> { "C#", ".NET 8" },
        Year = DateTime.Now.Year,
        IsFeatured = false,
        Status = 0
    };
}