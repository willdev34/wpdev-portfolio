// Título: SkillNetworkLayoutTests.cs
// Descrição: Testes unitários do SkillNetworkLayout (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class SkillNetworkLayoutTests
{
    [Theory]
    [InlineData(0, SkillNetworkSide.Right, SkillNetworkSide.Left)]
    [InlineData(1, SkillNetworkSide.Left, SkillNetworkSide.Right)]
    [InlineData(2, SkillNetworkSide.Right, SkillNetworkSide.Left)]
    [InlineData(7, SkillNetworkSide.Left, SkillNetworkSide.Right)]
    public void Lados_DevemAlternarPeloIndiceERedeFicaNoLadoOpostoAoCard(int index, SkillNetworkSide card, SkillNetworkSide rede)
    {
        SkillNetworkLayout.CardSide(index).Should().Be(card);
        SkillNetworkLayout.NetworkSide(index).Should().Be(rede);
    }

    [Fact]
    public void Compute_ZeroOuNegativo_DeveRetornarListaVazia()
    {
        SkillNetworkLayout.Compute(0, SkillNetworkSide.Left, 0).Should().BeEmpty();
        SkillNetworkLayout.Compute(0, SkillNetworkSide.Left, -3).Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Compute_DeveRetornarUmaPosicaoPorHabilidade(int count)
    {
        SkillNetworkLayout.Compute(2, SkillNetworkSide.Left, count).Should().HaveCount(count);
    }

    [Theory]
    [InlineData(0, SkillNetworkSide.Left, 5)]
    [InlineData(3, SkillNetworkSide.Right, 4)]
    public void Compute_MesmaEntrada_DeveGerarMesmaSaida(int index, SkillNetworkSide side, int count)
    {
        var primeira = SkillNetworkLayout.Compute(index, side, count);
        var segunda = SkillNetworkLayout.Compute(index, side, count);

        segunda.Should().Equal(primeira);
    }

    [Fact]
    public void Compute_EventosDiferentes_DevemTerFasesDiferentes()
    {
        var a = SkillNetworkLayout.Compute(0, SkillNetworkSide.Left, 3);
        var b = SkillNetworkLayout.Compute(1, SkillNetworkSide.Left, 3);

        a.Select(p => p.Phase).Should().NotEqual(b.Select(p => p.Phase));
    }

    [Theory]
    [InlineData(SkillNetworkSide.Left, 1)]
    [InlineData(SkillNetworkSide.Right, 3)]
    [InlineData(SkillNetworkSide.Left, 5)]
    public void Compute_PosicoesDoMesmoEvento_DevemSerDistintasEEspacadasPorAltura(SkillNetworkSide side, int count)
    {
        var posicoes = SkillNetworkLayout.Compute(4, side, count);

        posicoes.Select(p => (p.X, p.Y)).Distinct().Should().HaveCount(count);

        var alturas = posicoes.Select(p => p.Y).OrderBy(y => y).ToList();
        for (var i = 1; i < alturas.Count; i++)
        {
            (alturas[i] - alturas[i - 1]).Should().BeGreaterThanOrEqualTo(SkillNetworkLayout.SlotHeight);
        }
    }

    [Theory]
    [InlineData(SkillNetworkSide.Left)]
    [InlineData(SkillNetworkSide.Right)]
    public void Compute_Amplitude_DeveFicarEntre40E70(SkillNetworkSide side)
    {
        for (var index = 0; index < 8; index++)
        {
            foreach (var p in SkillNetworkLayout.Compute(index, side, 5))
            {
                p.Amplitude.Should().BeInRange(40, 70);
            }
        }
    }

    [Fact]
    public void Compute_RedeEsquerda_DeveFicarDoLadoNegativoComMargemDaLinhaCentral()
    {
        foreach (var p in SkillNetworkLayout.Compute(1, SkillNetworkSide.Left, 5))
        {
            p.X.Should().BeLessThanOrEqualTo(-SkillNetworkLayout.MinX);
        }
    }

    [Fact]
    public void Compute_RedeDireita_DeveFicarDoLadoPositivoComMargemDaLinhaCentral()
    {
        foreach (var p in SkillNetworkLayout.Compute(0, SkillNetworkSide.Right, 5))
        {
            p.X.Should().BeGreaterThanOrEqualTo(SkillNetworkLayout.MinX);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Compute_DeveCaberNaAlturaRequeridaComMargemEmCimaEEmbaixo(int count)
    {
        var limite = SkillNetworkLayout.RequiredHeight(count) / 2 - SkillNetworkLayout.VerticalMargin;

        foreach (var p in SkillNetworkLayout.Compute(0, SkillNetworkSide.Left, count))
        {
            // O slot da bolha tem SlotHeight; metade dele fica de cada lado do centro
            (Math.Abs(p.Y) + SkillNetworkLayout.SlotHeight / 2).Should().BeLessThanOrEqualTo(limite);
        }
    }

    [Fact]
    public void Compute_AngulosEDistanciasDevemSerCoerentesComXEY()
    {
        foreach (var p in SkillNetworkLayout.Compute(0, SkillNetworkSide.Left, 5))
        {
            // Arredondamentos de 1 px em X e Y e de 0,1 grau no ângulo
            var radianos = p.Angle * Math.PI / 180;
            (Math.Cos(radianos) * p.Distance).Should().BeApproximately(p.X, 2);
            (Math.Sin(radianos) * p.Distance).Should().BeApproximately(p.Y, 2);
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-2, 0)]
    public void RequiredHeight_SemHabilidades_DeveSerZero(int count, double esperado)
    {
        SkillNetworkLayout.RequiredHeight(count).Should().Be(esperado);
    }

    [Theory]
    [InlineData(1, 142)]
    [InlineData(5, 390)]
    public void RequiredHeight_DeveSomarSlotsEMargens(int count, double esperado)
    {
        SkillNetworkLayout.RequiredHeight(count).Should().Be(esperado);
    }
}
