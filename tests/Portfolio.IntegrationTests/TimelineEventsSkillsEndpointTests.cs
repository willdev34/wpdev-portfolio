// ====================================
// Título: TimelineEventsSkillsEndpointTests.cs
// Descrição: Testes de integração das habilidades dos eventos da timeline: leitura (lista e por id),
//            criação com 5 e rejeição de 6, regras de validação no PUT, semântica do campo skills
//            (omitido ou nulo mantém, [] limpa, lista substitui), 401 nos endpoints de escrita e
//            a cascata no banco ao remover o evento.
//
//            Cada teste cria os próprios eventos (título único, datas no século 19) e filtra o
//            resultado por eles, sem assertar contagem total.
// ====================================

using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Infrastructure.Data;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class TimelineEventsSkillsEndpointTests : IntegrationTestBase
{
    private const string Url = "/api/timelineevents";
    private readonly CustomWebApplicationFactory _factory;

    public TimelineEventsSkillsEndpointTests(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    // ------------------------------------------------------------------
    // Leitura
    // ------------------------------------------------------------------

    [Fact]
    public async Task Get_List_ReturnsSkillsInOrderForEachEvent()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("Terceira", "Stack"), ("Primeira", "HardSkill"), ("Segunda", "SoftSkill")));
        ClearAuthentication();

        var all = await Client.GetFromJsonAsync<List<TimelineEventCardDto>>(Url);

        var ours = all!.Single(e => e.Id == created.Id);
        ours.Skills.Select(s => s.Name).Should().Equal("Terceira", "Primeira", "Segunda");
        ours.Skills.Select(s => s.Category).Should().Equal("Stack", "HardSkill", "SoftSkill");
        ours.Skills.Select(s => s.Order).Should().Equal(0, 1, 2);
    }

    [Fact]
    public async Task Get_ById_ReturnsSkills()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("Comunicação", "SoftSkill")));

        var got = await Client.GetFromJsonAsync<TimelineEventDto>($"{Url}/{created.Id}");

        got!.Skills.Select(s => s.Name).Should().Equal("C#", "Comunicação");
        got.Skills.Should().OnlyContain(s => s.Id != Guid.Empty);
    }

    [Fact]
    public async Task Get_List_IsInChronologicalOrder()
    {
        await AuthenticateClientAsync();
        var unique = NewUnique();
        // Criados fora de ordem cronológica
        var middle = await CreateAsync(null, title: $"Meio {unique}", date: Utc(1871, 6, 1));
        var last = await CreateAsync(null, title: $"Fim {unique}", date: Utc(1871, 12, 1));
        var first = await CreateAsync(null, title: $"Início {unique}", date: Utc(1871, 1, 1));
        ClearAuthentication();

        var all = await Client.GetFromJsonAsync<List<TimelineEventCardDto>>(Url);

        all!.Where(e => e.Title.EndsWith(unique)).Select(e => e.Id)
            .Should().Equal(first.Id, middle.Id, last.Id);
    }

    // ------------------------------------------------------------------
    // Criação
    // ------------------------------------------------------------------

    [Fact]
    public async Task Post_Create_WithFiveSkills_ReturnsCreatedWithTrimmedNames()
    {
        await AuthenticateClientAsync();
        var dto = BuildCreateDto(Skills(("  C#  ", "Stack"), ("SQL", "HardSkill"), ("Docker", "stack"), ("Git", "Stack"), ("Liderança", "SoftSkill")));

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<TimelineEventDto>();
        created!.Skills.Select(s => s.Name).Should().Equal("C#", "SQL", "Docker", "Git", "Liderança");
        created.Skills.Select(s => s.Category).Should().Equal("Stack", "HardSkill", "Stack", "Stack", "SoftSkill");
    }

    [Fact]
    public async Task Post_Create_WithSixSkills_ReturnsBadRequest()
    {
        await AuthenticateClientAsync();
        var dto = BuildCreateDto(Enumerable.Range(1, 6).Select(i => new TimelineEventSkillInputDto { Name = $"Skill {i}", Category = "Stack" }).ToList());

        var response = await Client.PostAsJsonAsync(Url, dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Create_WithoutSkills_ReturnsCreatedWithEmptyList()
    {
        await AuthenticateClientAsync();

        var created = await CreateAsync(null);

        created.Skills.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(InvalidSkillLists))]
    public async Task Post_Create_WithInvalidSkills_ReturnsBadRequest(string caso, List<TimelineEventSkillInputDto> skills)
    {
        await AuthenticateClientAsync();

        var response = await Client.PostAsJsonAsync(Url, BuildCreateDto(skills));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because: caso);
    }

    public static IEnumerable<object[]> InvalidSkillLists()
    {
        yield return new object[] { "nome vazio", Skills(("", "Stack")) };
        yield return new object[] { "nome só com espaços", Skills(("   ", "Stack")) };
        yield return new object[] { "nome com 41 caracteres", Skills((new string('a', 41), "Stack")) };
        yield return new object[] { "duplicado ignorando maiúsculas e espaços", Skills(("Go", "Stack"), (" gO ", "Stack")) };
        yield return new object[] { "categoria inexistente", Skills(("Go", "Banana")) };
        yield return new object[] { "categoria numérica", Skills(("Go", "1")) };
    }

    // ------------------------------------------------------------------
    // PUT: skills omitido ou nulo mantém, [] limpa, lista substitui
    // ------------------------------------------------------------------

    [Fact]
    public async Task Put_WithSkillsOmitted_KeepsCurrentSkills()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        var status = await PutRawAsync(created.Id, created.Title + " editado", skillsJson: null);

        status.Should().Be(HttpStatusCode.NoContent);
        var got = await GetAsync(created.Id);
        got.Title.Should().EndWith("editado");
        got.Skills.Select(s => s.Name).Should().Equal("C#", "SQL");
    }

    [Fact]
    public async Task Put_WithSkillsNull_KeepsCurrentSkills()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        var status = await PutRawAsync(created.Id, created.Title, skillsJson: "null");

        status.Should().Be(HttpStatusCode.NoContent);
        (await GetAsync(created.Id)).Skills.Select(s => s.Name).Should().Equal("C#", "SQL");
    }

    [Fact]
    public async Task Put_WithEmptySkillsList_ClearsSkills()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        var status = await PutRawAsync(created.Id, created.Title, skillsJson: "[]");

        status.Should().Be(HttpStatusCode.NoContent);
        (await GetAsync(created.Id)).Skills.Should().BeEmpty();
    }

    [Fact]
    public async Task Put_WithSkillsList_ReplacesSkillsAndOrderFollowsListPosition()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        var status = await PutRawAsync(created.Id, created.Title,
            skillsJson: """[{"name":"Docker","category":"Stack"},{"name":" Liderança ","category":"SoftSkill"},{"name":"Go","category":"Stack"}]""");

        status.Should().Be(HttpStatusCode.NoContent);
        var got = await GetAsync(created.Id);
        got.Skills.Select(s => s.Name).Should().Equal("Docker", "Liderança", "Go");
        got.Skills.Select(s => s.Order).Should().Equal(0, 1, 2);
    }

    [Fact]
    public async Task Put_ReplacingSkillsTwice_DoesNotLeaveOldRowsInTheDatabase()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        await PutRawAsync(created.Id, created.Title, """[{"name":"Go","category":"Stack"}]""");
        await PutRawAsync(created.Id, created.Title, """[{"name":"Rust","category":"Stack"},{"name":"Git","category":"Stack"}]""");

        var rows = await SkillNamesInDatabaseAsync(created.Id);
        rows.Should().BeEquivalentTo(new[] { "Rust", "Git" });
    }

    [Fact]
    public async Task Put_WithSixSkills_ReturnsBadRequestAndKeepsCurrentSkills()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack")));
        var six = string.Join(",", Enumerable.Range(1, 6).Select(i => $$"""{"name":"S{{i}}","category":"Stack"}"""));

        var status = await PutRawAsync(created.Id, created.Title, $"[{six}]");

        status.Should().Be(HttpStatusCode.BadRequest);
        (await GetAsync(created.Id)).Skills.Select(s => s.Name).Should().Equal("C#");
    }

    [Fact]
    public async Task Put_WithDuplicatedSkills_ReturnsBadRequest()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(null);

        var status = await PutRawAsync(created.Id, created.Title,
            """[{"name":"Go","category":"Stack"},{"name":"go","category":"HardSkill"}]""");

        status.Should().Be(HttpStatusCode.BadRequest);
    }

    // ------------------------------------------------------------------
    // 401 sem token nos endpoints de escrita
    // ------------------------------------------------------------------

    [Fact]
    public async Task Post_WithoutToken_ReturnsUnauthorized()
    {
        var response = await Client.PostAsJsonAsync(Url, BuildCreateDto(Skills(("C#", "Stack"))));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_WithoutToken_ReturnsUnauthorized()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack")));
        ClearAuthentication();

        var status = await PutRawAsync(created.Id, created.Title, """[]""");

        status.Should().Be(HttpStatusCode.Unauthorized);
        await AuthenticateClientAsync();
        (await GetAsync(created.Id)).Skills.Should().HaveCount(1);
    }

    [Fact]
    public async Task Delete_WithoutToken_ReturnsUnauthorized()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack")));
        ClearAuthentication();

        var response = await Client.DeleteAsync($"{Url}/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ------------------------------------------------------------------
    // Remoção: soft delete mantém as habilidades; a cascata vale ao remover a linha do evento
    // ------------------------------------------------------------------

    [Fact]
    public async Task Delete_ViaApi_IsSoftDeleteAndKeepsTheSkillsInTheDatabase()
    {
        await AuthenticateClientAsync();
        var created = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill")));

        var response = await Client.DeleteAsync($"{Url}/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await SkillNamesInDatabaseAsync(created.Id)).Should().BeEquivalentTo(new[] { "C#", "SQL" });
    }

    [Fact]
    public async Task RemovingTheEventRowDirectly_CascadesToItsSkills_AndOnlyToIt()
    {
        await AuthenticateClientAsync();
        var target = await CreateAsync(Skills(("C#", "Stack"), ("SQL", "HardSkill"), ("Go", "Stack")));
        var other = await CreateAsync(Skills(("Docker", "Stack")));

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
            // DELETE direto no banco (sem carregar as habilidades no EF): quem apaga as filhas é a FK em cascata
            var removed = await context.TimelineEvents.Where(e => e.Id == target.Id).ExecuteDeleteAsync();
            removed.Should().Be(1);
        }

        (await SkillNamesInDatabaseAsync(target.Id)).Should().BeEmpty();
        (await SkillNamesInDatabaseAsync(other.Id)).Should().BeEquivalentTo(new[] { "Docker" });
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private async Task<TimelineEventDto> CreateAsync(
        List<TimelineEventSkillInputDto>? skills, string? title = null, DateTime? date = null)
    {
        var dto = BuildCreateDto(skills ?? new List<TimelineEventSkillInputDto>(), title, date);
        var response = await Client.PostAsJsonAsync(Url, dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<TimelineEventDto>())!;
    }

    private async Task<TimelineEventDto> GetAsync(Guid id) =>
        (await Client.GetFromJsonAsync<TimelineEventDto>($"{Url}/{id}"))!;

    /// <summary>
    /// PUT com JSON montado à mão, para controlar se o campo skills vai omitido, nulo, vazio ou preenchido
    /// </summary>
    private async Task<HttpStatusCode> PutRawAsync(Guid id, string title, string? skillsJson)
    {
        var skillsPart = skillsJson == null ? string.Empty : $",\"skills\":{skillsJson}";
        var body = $$"""
            {"id":"{{id}}","title":"{{title}}","description":"Descrição gerada pelo teste de integração das habilidades.",
             "date":"1870-01-01T00:00:00Z","type":1,"isVisible":true{{skillsPart}}}
            """;
        var response = await Client.PutAsync($"{Url}/{id}", new StringContent(body, Encoding.UTF8, "application/json"));
        return response.StatusCode;
    }

    private async Task<List<string>> SkillNamesInDatabaseAsync(Guid eventId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        return await context.TimelineEventSkills
            .Where(s => s.TimelineEventId == eventId)
            .Select(s => s.Name)
            .ToListAsync();
    }

    private void ClearAuthentication() => Client.DefaultRequestHeaders.Authorization = null;

    private static string NewUnique() => Guid.NewGuid().ToString("N")[..8];

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    private static List<TimelineEventSkillInputDto> Skills(params (string Name, string Category)[] items) =>
        items.Select(i => new TimelineEventSkillInputDto { Name = i.Name, Category = i.Category }).ToList();

    private static CreateTimelineEventDto BuildCreateDto(
        List<TimelineEventSkillInputDto> skills, string? title = null, DateTime? date = null) => new()
    {
        Title = title ?? $"Evento de Habilidades {NewUnique()}",
        Description = "Descrição gerada pelo teste de integração das habilidades.",
        Date = date ?? Utc(1870, 1, 1),
        Type = 1,
        IsVisible = true,
        Skills = skills
    };
}
