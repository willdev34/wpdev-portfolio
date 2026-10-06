// Título: ProjectStatusHelperTests.cs
// Descrição: Testes unitários do ProjectStatusHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Domain.Entities;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class ProjectStatusHelperTests
{
    [Fact]
    public void Options_DeveCobrirTodosOsValoresDoEnumDoDominio_ComOMesmoNomeENumero()
    {
        // Se alguém criar um status novo no enum e esquecer do select do admin, este teste falha
        var doDominio = Enum.GetValues<ProjectStatus>().Select(s => ((int)s, s.ToString())).ToList();

        var doHelper = ProjectStatusHelper.Options.Select(o => (o.Value, o.Name)).ToList();

        doHelper.Should().BeEquivalentTo(doDominio);
    }

    [Fact]
    public void Options_DeveTerRotulosEmPortuguesSemRepeticao()
    {
        ProjectStatusHelper.Options.Select(o => o.Label)
            .Should().Equal("Planejamento", "Em andamento", "Concluído", "Arquivado");
        ProjectStatusHelper.Options.Select(o => o.Value).Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData("Planning", 0)]
    [InlineData("InProgress", 1)]
    [InlineData("Completed", 2)]
    [InlineData("Archived", 3)]
    public void TryParse_DeveConverterONomeDaApiNoValorDoFormulario(string nome, int esperado)
    {
        ProjectStatusHelper.TryParse(nome, out var valor).Should().BeTrue();

        valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData("inprogress", 1)]
    [InlineData("COMPLETED", 2)]
    [InlineData("  Archived ", 3)]
    public void TryParse_DeveIgnorarMaiusculasEEspacosDasPontas(string nome, int esperado)
    {
        ProjectStatusHelper.TryParse(nome, out var valor).Should().BeTrue();

        valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Paused")]
    [InlineData("Pausado")]
    [InlineData("2")]
    [InlineData("In Progress")]
    public void TryParse_DeveFalhar_ParaNomeDesconhecidoVazioOuNumerico(string? nome)
    {
        ProjectStatusHelper.TryParse(nome, out var valor).Should().BeFalse();

        valor.Should().Be(ProjectStatusHelper.Unknown);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(-1, false)]
    [InlineData(4, false)]
    public void IsValid_DeveAceitarSoOsValoresDasOpcoes(int valor, bool esperado)
    {
        ProjectStatusHelper.IsValid(valor).Should().Be(esperado);
    }

    [Fact]
    public void Unknown_NaoPodeSerUmValorValido()
    {
        ProjectStatusHelper.IsValid(ProjectStatusHelper.Unknown).Should().BeFalse();
    }

    [Theory]
    [InlineData(0, "Planejamento")]
    [InlineData(1, "Em andamento")]
    [InlineData(2, "Concluído")]
    [InlineData(3, "Arquivado")]
    [InlineData(-1, "")]
    [InlineData(9, "")]
    public void Label_DeveDevolverORotulo_OuVazioParaValorDesconhecido(int valor, string esperado)
    {
        ProjectStatusHelper.Label(valor).Should().Be(esperado);
    }

    [Fact]
    public void NomeDoEnum_DeveSobreviverAIdaEVolta_ParaTodosOsStatus()
    {
        foreach (var status in Enum.GetValues<ProjectStatus>())
        {
            ProjectStatusHelper.TryParse(status.ToString(), out var valor).Should().BeTrue(status.ToString());

            valor.Should().Be((int)status);
        }
    }
}
