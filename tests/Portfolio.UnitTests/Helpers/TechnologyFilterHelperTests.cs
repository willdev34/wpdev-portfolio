// Título: TechnologyFilterHelperTests.cs
// Descrição: Testes unitários do TechnologyFilterHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class TechnologyFilterHelperTests
{
    [Fact]
    public void BuildAvailable_DeveMostrarCadaTecnologiaUmaVez_ApesarDasVariacoes()
    {
        var porProjeto = new[]
        {
            new[] { "CSS", "Javascript", "HTML 5" },
            new[] { "CSS3", "JavaScript", "HTML5", "React" },
            new[] { "css 3", "JS", "html" }
        };

        var disponiveis = TechnologyFilterHelper.BuildAvailable(porProjeto);

        disponiveis.Should().Equal("CSS", "HTML", "JavaScript", "React");
    }

    [Fact]
    public void BuildAvailable_DeveManterVersoesComoTecnologiasSeparadas()
    {
        var disponiveis = TechnologyFilterHelper.BuildAvailable(new[] { new[] { "Next.js", "Next.js 16", "Django 4.2" } });

        disponiveis.Should().Equal("Django 4.2", "Next.js", "Next.js 16");
    }

    [Fact]
    public void BuildAvailable_DeveDevolverVazio_QuandoNaoHaProjetos()
    {
        TechnologyFilterHelper.BuildAvailable(Array.Empty<string[]>()).Should().BeEmpty();
    }

    [Fact]
    public void MatchesAny_DeveCasarVariacoesDoMesmoNome()
    {
        TechnologyFilterHelper.MatchesAny(new[] { "HTML5", "CSS 3" }, new[] { "CSS" }).Should().BeTrue();
        TechnologyFilterHelper.MatchesAny(new[] { "Javascript" }, new[] { "JavaScript" }).Should().BeTrue();
    }

    [Fact]
    public void MatchesAny_DeveUsarCasamentoExato_NaoParcial()
    {
        TechnologyFilterHelper.MatchesAny(new[] { "JavaScript" }, new[] { "Java" }).Should().BeFalse();
        TechnologyFilterHelper.MatchesAny(new[] { "Next.js 16" }, new[] { "Next.js" }).Should().BeFalse();
        TechnologyFilterHelper.MatchesAny(new[] { "C#" }, new[] { "C" }).Should().BeFalse();
    }

    [Fact]
    public void MatchesAny_DevePassarQualquerProjeto_QuandoNaoHaSelecao()
    {
        TechnologyFilterHelper.MatchesAny(new[] { "Docker" }, Array.Empty<string>()).Should().BeTrue();
        TechnologyFilterHelper.MatchesAny(Array.Empty<string>(), Array.Empty<string>()).Should().BeTrue();
    }

    [Fact]
    public void MatchesAny_DevePassar_QuandoCasaQualquerUmaDasSelecionadas()
    {
        TechnologyFilterHelper.MatchesAny(new[] { "Python", "Django" }, new[] { "React", "Django" }).Should().BeTrue();
        TechnologyFilterHelper.MatchesAny(new[] { "Python" }, new[] { "React", "Django" }).Should().BeFalse();
    }

    [Theory]
    [InlineData("CSS 3", "CSS")]
    [InlineData("css3", "CSS")]
    [InlineData("html 5", "HTML")]
    [InlineData("javascript", "JavaScript")]
    [InlineData("react", "React")]
    public void ResolveFromQuery_DeveAceitarLinksAntigos_ERetornarONomeDoChip(string consulta, string esperado)
    {
        var disponiveis = new[] { "CSS", "HTML", "JavaScript", "React" };

        TechnologyFilterHelper.ResolveFromQuery(consulta, disponiveis).Should().Be(esperado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Cobol")]
    public void ResolveFromQuery_DeveDevolverNulo_QuandoNaoHaEquivalente(string? consulta)
    {
        TechnologyFilterHelper.ResolveFromQuery(consulta, new[] { "CSS", "React" }).Should().BeNull();
    }
}
