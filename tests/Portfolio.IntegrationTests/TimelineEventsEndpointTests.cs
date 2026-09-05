// ====================================
// Título: TimelineEventsEndpointTests.cs
// Descrição: Testes de integração do TimelineEventsController - CRUD e
//            soft-delete via IsVisible. Inclui teste de regressão para
//            o bug historico onde editar um evento zerava IsVisible.
// ====================================

using System.Net;
using System.Net.Http.Json;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class TimelineEventsEndpointTests : IntegrationTestBase
{
    public TimelineEventsEndpointTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_AllTimelineEvents_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/timelineevents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_CreateTimelineEvent_WithoutToken_ReturnsUnauthorized()
    {
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/timelineevents", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_CreateTimelineEvent_WithToken_ReturnsCreated_AndVisible()
    {
        await AuthenticateClientAsync();
        var dto = BuildValidCreateDto();

        var response = await Client.PostAsJsonAsync("/api/timelineevents", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<TimelineEventDto>();
        created.Should().NotBeNull();
        created!.IsVisible.Should().BeTrue();
    }

    [Fact]
    public async Task Put_UpdateTimelineEvent_WithIsVisibleTrue_StaysVisibleInGetAll()
    {
        // Teste de regressao: no Sprint 10, editar um evento zerava IsVisible
        // silenciosamente porque o campo estava ausente nos DTOs do Web,
        // e como IsVisible tambem funciona como soft-delete, toda edicao
        // "apagava" o evento sem ninguem perceber. Esse teste garante que
        // um update explicito com IsVisible=true nunca regride esse comportamento.
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/timelineevents", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<TimelineEventDto>();

        var updateDto = new UpdateTimelineEventDto
        {
            Id = created!.Id,
            Title = "Evento Atualizado via Teste",
            Description = createDto.Description,
            Date = createDto.Date,
            Type = createDto.Type,
            Order = createDto.Order,
            IsVisible = true
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/timelineevents/{created.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var allResponse = await Client.GetAsync("/api/timelineevents");
        var all = await allResponse.Content.ReadFromJsonAsync<List<TimelineEventCardDto>>();

        all.Should().Contain(e => e.Id == created.Id);
    }

    [Fact]
    public async Task Delete_TimelineEvent_RemovesFromGetAll_ButStillReturnsFromGetById()
    {
        // Diferente de Projects e BlogPosts: o GetByIdAsync do TimelineEvent
        // nao filtra por IsVisible, entao o soft-delete some da listagem
        // publica (GetAll) mas o registro continua acessivel por ID,
        // com IsVisible = false. Esse teste documenta esse comportamento
        // especifico pra ele nao ser confundido com um bug no futuro.
        await AuthenticateClientAsync();
        var createDto = BuildValidCreateDto();
        var createResponse = await Client.PostAsJsonAsync("/api/timelineevents", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<TimelineEventDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/timelineevents/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var allResponse = await Client.GetAsync("/api/timelineevents");
        var all = await allResponse.Content.ReadFromJsonAsync<List<TimelineEventCardDto>>();
        all.Should().NotContain(e => e.Id == created.Id);

        var getByIdResponse = await Client.GetAsync($"/api/timelineevents/{created.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var stillThere = await getByIdResponse.Content.ReadFromJsonAsync<TimelineEventDto>();
        stillThere!.IsVisible.Should().BeFalse();
    }

    private static CreateTimelineEventDto BuildValidCreateDto() => new()
    {
        Title = $"Evento de Teste {Guid.NewGuid()}",
        Description = "Descrição gerada pelo teste de integração, com mais de dez caracteres.",
        Date = DateTime.UtcNow.Date,
        Type = 1, // Work
        Order = 0,
        IsVisible = true
    };
}