// Título: TechnologyNormalizerTests.cs
// Descrição: Testes unitários da regra de normalização de nomes de tecnologia

using FluentAssertions;
using Portfolio.Application.Helpers;

namespace Portfolio.UnitTests.Projects;

public class TechnologyNormalizerTests
{
    [Theory]
    [InlineData("CSS3", "CSS")]
    [InlineData("CSS 3", "CSS")]
    [InlineData("css", "CSS")]
    [InlineData("HTML5", "HTML")]
    [InlineData("HTML 5", "HTML")]
    [InlineData("html", "HTML")]
    [InlineData("Javascript", "JavaScript")]
    [InlineData("JS", "JavaScript")]
    [InlineData("js", "JavaScript")]
    [InlineData("JavaScript (Vanilla JS)", "JavaScript")]
    [InlineData("Github Pages", "GitHub Pages")]
    [InlineData("github  pages", "GitHub Pages")]
    public void Normalize_DeveDevolverONomeCanonico_QuandoEstiverNoDicionario(string entrada, string esperado)
    {
        TechnologyNormalizer.Normalize(entrada).Should().Be(esperado);
    }

    [Theory]
    [InlineData("Next.js 16")]
    [InlineData("Django 4.2")]
    [InlineData("Prisma 7")]
    [InlineData("GitHub Actions")]
    [InlineData("Unity 5")]
    [InlineData(".NET 8")]
    [InlineData("C#")]
    public void Normalize_DeveManterONome_QuandoNaoEstiverNoDicionario(string entrada)
    {
        TechnologyNormalizer.Normalize(entrada).Should().Be(entrada);
    }

    [Fact]
    public void Normalize_DeveRemoverEspacosDasPontasEColapsarOsInternos()
    {
        TechnologyNormalizer.Normalize("  Tailwind    CSS  ").Should().Be("Tailwind CSS");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_DeveDevolverVazio_QuandoNaoHaTexto(string? entrada)
    {
        TechnologyNormalizer.Normalize(entrada).Should().BeEmpty();
    }

    [Fact]
    public void Key_DeveIgnorarMaiusculasEEspacos()
    {
        TechnologyNormalizer.Key("CSS 3").Should().Be("css3");
        TechnologyNormalizer.Key(" Next.js  16 ").Should().Be("next.js16");
        TechnologyNormalizer.Key(null).Should().BeEmpty();
    }

    [Fact]
    public void NormalizeAll_DeveUnificarVariacoes_MantendoPrimeiraOcorrenciaEOrdem()
    {
        var entrada = new[] { "React", "CSS3", "css 3", "CSS", "Javascript", "JS", "HTML 5", "HTML5", "React" };

        var resultado = TechnologyNormalizer.NormalizeAll(entrada);

        resultado.Should().Equal("React", "CSS", "JavaScript", "HTML");
    }

    [Fact]
    public void NormalizeAll_DeveDescartarVaziosENulos()
    {
        var resultado = TechnologyNormalizer.NormalizeAll(new string?[] { " ", null, "", "Docker" });

        resultado.Should().Equal("Docker");
    }

    [Fact]
    public void NormalizeAll_DeveSepararItensComVirgula_PoisOArmazenamentoUsaVirgulaComoSeparador()
    {
        var resultado = TechnologyNormalizer.NormalizeAll(new[] { "C#, F#", "c#" });

        resultado.Should().Equal("C#", "F#");
    }

    [Fact]
    public void NormalizeAll_DeveDevolverListaVazia_QuandoEntradaForNula()
    {
        TechnologyNormalizer.NormalizeAll(null).Should().BeEmpty();
    }

    [Fact]
    public void NormalizeAll_DeveSerIdempotente()
    {
        var primeira = TechnologyNormalizer.NormalizeAll(new[] { "Javascript", "CSS 3", "Github Pages", "Next.js 16" });

        var segunda = TechnologyNormalizer.NormalizeAll(primeira);

        segunda.Should().Equal(primeira);
    }

    [Theory]
    [InlineData("CSS 3", "css", true)]
    [InlineData("HTML5", "html 5", true)]
    [InlineData("Javascript", "JavaScript", true)]
    [InlineData("Next.js", "Next.js 16", false)]
    [InlineData("Java", "JavaScript", false)]
    [InlineData("", "", false)]
    [InlineData(null, "CSS", false)]
    public void AreEquivalent_DeveCompararPelaChaveCanonica_ComCasamentoExato(string? a, string? b, bool esperado)
    {
        TechnologyNormalizer.AreEquivalent(a, b).Should().Be(esperado);
    }
}
