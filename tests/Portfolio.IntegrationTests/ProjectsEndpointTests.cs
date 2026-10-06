using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Web.Helpers;

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
    public async Task Get_FeaturedProjects_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/projects/featured");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectCardDto>>();
        projects.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_FeaturedProjects_ReturnsOnlyFeaturedOnes()
    {
        await AuthenticateClientAsync();

        var featuredDto = BuildValidCreateDto();
        featuredDto.IsFeatured = true;
        await Client.PostAsJsonAsync("/api/projects", featuredDto);

        var notFeaturedDto = BuildValidCreateDto();
        notFeaturedDto.IsFeatured = false;
        await Client.PostAsJsonAsync("/api/projects", notFeaturedDto);

        var response = await Client.GetAsync("/api/projects/featured");
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectCardDto>>();

        projects.Should().NotBeNull();
        projects.Should().Contain(p => p.Title == featuredDto.Title);
        projects.Should().NotContain(p => p.Title == notFeaturedDto.Title);
        projects.Should().OnlyContain(p => p.IsFeatured);
    }

    [Fact]
    public async Task Get_FeaturedProjects_DoesNotRequireAuthentication()
    {
        // Sem AuthenticateClientAsync() de propósito
        var response = await Client.GetAsync("/api/projects/featured");

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
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

    // Regressão do bug do admin: o formulário abria sempre com "Concluído" e salvar sobrescrevia o status real.
    // Aqui o ciclo é o do admin: ler (a API devolve o NOME), converter pelo helper do Web (o número que a API
    // espera), atualizar sem mexer em nada e ler de novo. Nada pode mudar, para nenhum status.
    [Theory]
    [InlineData(0, "Planning")]
    [InlineData(1, "InProgress")]
    [InlineData(2, "Completed")]
    [InlineData(3, "Archived")]
    public async Task Put_SalvarSemAlterar_DeveManterOProjetoInteiroIgual_ParaCadaStatus(int status, string nomeEsperado)
    {
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        createDto.Status = status;
        createDto.IsFeatured = true;
        createDto.ShortDescription = "Resumo curto do projeto";
        createDto.DemoUrl = "https://exemplo.com/demo";
        createDto.RepositoryUrl = "https://github.com/exemplo/repo";

        var createResponse = await Client.PostAsJsonAsync("/api/projects", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();
        created!.Status.Should().Be(nomeEsperado);

        var antes = (await (await Client.GetAsync($"/api/projects/{created.Id}")).Content.ReadFromJsonAsync<ProjectDto>())!;
        ProjectStatusHelper.TryParse(antes.Status, out var numeroDoHelper).Should().BeTrue();
        numeroDoHelper.Should().Be(status);

        // O que o admin faz ao salvar sem mexer: devolve tudo como veio, com o status convertido pelo helper
        var updateDto = new UpdateProjectDto
        {
            Id = antes.Id,
            Title = antes.Title,
            Description = antes.Description,
            ShortDescription = antes.ShortDescription,
            ImageUrl = antes.ImageUrl,
            DemoUrl = antes.DemoUrl,
            RepositoryUrl = antes.RepositoryUrl,
            Technologies = antes.Technologies,
            Year = antes.Year,
            IsFeatured = antes.IsFeatured,
            Status = numeroDoHelper,
            IsActive = antes.IsActive
        };
        var updateResponse = await Client.PutAsJsonAsync($"/api/projects/{antes.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var depois = (await (await Client.GetAsync($"/api/projects/{antes.Id}")).Content.ReadFromJsonAsync<ProjectDto>())!;
        depois.Status.Should().Be(nomeEsperado);
        depois.Should().BeEquivalentTo(antes, options => options.Excluding(p => p.UpdatedAt));
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