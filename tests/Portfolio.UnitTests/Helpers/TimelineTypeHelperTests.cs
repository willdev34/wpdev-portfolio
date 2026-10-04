// Título: TimelineTypeHelperTests.cs
// Descrição: Testes unitários do TimelineTypeHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Domain.Entities;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class TimelineTypeHelperTests
{
    [Theory]
    [InlineData("Education", "Educação")]
    [InlineData("Work", "Trabalho")]
    [InlineData("Project", "Projeto")]
    [InlineData("Achievement", "Conquista")]
    [InlineData("Certification", "Certificação")]
    [InlineData("Other", "Outro")]
    public void Label_DeveRetornarRotuloEmPortugues(string type, string esperado)
    {
        TimelineTypeHelper.Label(type).Should().Be(esperado);
    }

    [Fact]
    public void Label_ValorDesconhecido_DeveRetornarTextoOriginal()
    {
        TimelineTypeHelper.Label("Hobby").Should().Be("Hobby");
    }

    [Fact]
    public void Label_Nulo_DeveRetornarVazio()
    {
        TimelineTypeHelper.Label(null).Should().BeEmpty();
    }

    [Fact]
    public void Options_DeveCobrirTodosOsValoresDoEnumComInteiroCorreto()
    {
        var enumValues = Enum.GetValues<TimelineEventType>();

        TimelineTypeHelper.Options.Should().HaveCount(enumValues.Length);
        foreach (var option in TimelineTypeHelper.Options)
        {
            Enum.Parse<TimelineEventType>(option.Name).Should().Be((TimelineEventType)option.Value);
        }
    }
}
