using System.Net;

namespace Portfolio.IntegrationTests;

public class HealthEndpointTests : IntegrationTestBase
{
    public HealthEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}