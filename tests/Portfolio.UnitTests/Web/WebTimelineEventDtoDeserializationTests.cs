// Título: WebTimelineEventDtoDeserializationTests.cs
// Descrição: Regressão da leitura do TimelineEventDto do Web (arquivo linkado no csproj, junto do
//            TimelineEventSkillDto que mora no mesmo arquivo): a API devolve nulos em EndDate,
//            IconUrl, LinkUrl e LinkText, e a lista de habilidades pode vir vazia ou preenchida

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using WebTimelineEventDto = Portfolio.Web.DTOs.Timeline.TimelineEventDto;

namespace Portfolio.UnitTests.Web;

/// <summary>
/// Contexto mínimo com source generator, nas mesmas opções de leitura do TimelineJsonContext do Web
/// </summary>
[JsonSerializable(typeof(WebTimelineEventDto))]
[JsonSerializable(typeof(List<WebTimelineEventDto>))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class WebTimelineTestJsonContext : JsonSerializerContext
{
}

public class WebTimelineEventDtoDeserializationTests
{
    [Fact]
    public void Deserialize_DeveLerEvento_QuandoEndDateIconUrlLinkUrlELinkTextNulos()
    {
        // Formato de GET api/timelineevents/{id} para um evento em andamento, sem ícone nem link
        const string json = """
        {
          "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
          "title": "Desenvolvedor Full Stack",
          "description": "Descrição do evento em andamento.",
          "date": "2025-07-01T00:00:00Z",
          "endDate": null,
          "isCurrent": true,
          "type": "Work",
          "iconUrl": null,
          "linkUrl": null,
          "linkText": null,
          "order": 0,
          "isVisible": true,
          "skills": [],
          "createdAt": "2026-10-01T12:00:00Z",
          "updatedAt": null
        }
        """;

        var act = () => JsonSerializer.Deserialize(json, WebTimelineTestJsonContext.Default.TimelineEventDto);

        var evento = act.Should().NotThrow().Subject;
        evento.Should().NotBeNull();
        evento!.EndDate.Should().BeNull();
        evento.IconUrl.Should().BeNull();
        evento.LinkUrl.Should().BeNull();
        evento.LinkText.Should().BeNull();
        evento.Type.Should().Be("Work");
        evento.IsCurrent.Should().BeTrue();
        evento.Skills.Should().BeEmpty();
    }

    [Fact]
    public void Deserialize_DeveLerHabilidadesNaOrdemRecebida()
    {
        const string json = """
        {
          "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
          "title": "Faculdade",
          "description": "Descrição do evento com habilidades.",
          "date": "2010-02-01T00:00:00Z",
          "endDate": "2013-12-01T00:00:00Z",
          "isCurrent": false,
          "type": "Education",
          "iconUrl": null,
          "linkUrl": null,
          "linkText": null,
          "order": 0,
          "isVisible": true,
          "skills": [
            { "id": "11111111-1111-1111-1111-111111111111", "name": "C#", "category": "Stack", "order": 0 },
            { "id": "22222222-2222-2222-2222-222222222222", "name": "Comunicação", "category": "SoftSkill", "order": 1 }
          ]
        }
        """;

        var evento = JsonSerializer.Deserialize(json, WebTimelineTestJsonContext.Default.TimelineEventDto);

        evento.Should().NotBeNull();
        evento!.Skills.Select(s => s.Name).Should().Equal("C#", "Comunicação");
        evento.Skills.Select(s => s.Category).Should().Equal("Stack", "SoftSkill");
        evento.Skills.Select(s => s.Order).Should().Equal(0, 1);
    }

    [Fact]
    public void Deserialize_DeveLerListaDaTimeline_QuandoItensTemCamposNulos()
    {
        // GET api/timelineevents devolve TimelineEventCardDto (sem linkUrl, linkText nem isVisible)
        const string json = """
        [
          {
            "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
            "title": "Primeiro contato com tecnologia",
            "description": "Descrição do primeiro evento.",
            "date": "2006-01-01T00:00:00Z",
            "endDate": null,
            "isCurrent": false,
            "type": "Education",
            "iconUrl": null,
            "order": 0,
            "skills": []
          },
          {
            "id": "4f2504e0-4f89-11d3-9a0c-0305e82c3302",
            "title": "Web Designer",
            "description": "Descrição do segundo evento.",
            "date": "2011-09-01T00:00:00Z",
            "endDate": "2012-12-01T00:00:00Z",
            "isCurrent": false,
            "type": "Work",
            "iconUrl": null,
            "order": 0,
            "skills": [ { "id": "11111111-1111-1111-1111-111111111111", "name": "HTML", "category": "HardSkill", "order": 0 } ]
          }
        ]
        """;

        var act = () => JsonSerializer.Deserialize(json, WebTimelineTestJsonContext.Default.ListTimelineEventDto);

        var lista = act.Should().NotThrow().Subject;
        lista.Should().HaveCount(2);
        lista![0].LinkUrl.Should().BeNull();
        lista[0].EndDate.Should().BeNull();
        lista[1].Skills.Should().ContainSingle().Which.Name.Should().Be("HTML");
    }
}
