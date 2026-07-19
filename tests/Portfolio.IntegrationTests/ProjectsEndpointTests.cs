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
}