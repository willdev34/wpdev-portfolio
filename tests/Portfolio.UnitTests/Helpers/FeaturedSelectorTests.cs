// Título: FeaturedSelectorTests.cs
// Descrição: Testes unitários do FeaturedSelector (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class FeaturedSelectorTests
{
    private sealed record Projeto(string Nome, DateTime CriadoEm);

    private static readonly DateTime Base = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // P0 é o mais recente, P1 o seguinte, e assim por diante
    private static List<Projeto> Lista(int quantidade) =>
        Enumerable.Range(0, quantidade).Select(i => new Projeto($"P{i}", Base.AddDays(-i))).ToList();

    private static FeaturedSelection<Projeto> Escolher(IEnumerable<Projeto> lista, int menores, int semente) =>
        FeaturedSelector.Pick(lista, p => p.CriadoEm, menores, semente);

    [Fact]
    public void Pick_DeveDarOMesmoResultado_ParaAMesmaSemente()
    {
        var lista = Lista(8);

        var primeira = Escolher(lista, 3, 12345);
        var segunda = Escolher(lista, 3, 12345);

        segunda.Primary.Should().Be(primeira.Primary);
        segunda.Secondary.Should().Equal(primeira.Secondary);
    }

    [Fact]
    public void Pick_DeveVariarOSorteio_ComSementesDiferentes()
    {
        var lista = Lista(8);

        var combinacoes = Enumerable.Range(1, 40)
            .Select(semente => string.Join(",", Escolher(lista, 3, semente).Secondary.Select(p => p.Nome)))
            .Distinct()
            .Count();

        combinacoes.Should().BeGreaterThan(5);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(-3)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Pick_NaoDeveRepetirProjetos(int semente)
    {
        var resultado = Escolher(Lista(8), 3, semente);

        var todos = resultado.Secondary.Append(resultado.Primary!).ToList();
        todos.Should().OnlyHaveUniqueItems();
        resultado.Secondary.Should().HaveCount(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(99)]
    [InlineData(-5)]
    public void Pick_PrincipalEhSempreOMaisRecente_ENuncaEntraNoSorteio(int semente)
    {
        var lista = Lista(6);

        var resultado = Escolher(lista, 5, semente);

        resultado.Primary!.Nome.Should().Be("P0");
        resultado.Secondary.Should().NotContain(resultado.Primary);
        resultado.Secondary.Should().HaveCount(5);
    }

    [Fact]
    public void Pick_DeveEscolherOPrincipalPelaData_NaoPelaOrdemDeEntrada()
    {
        var embaralhada = new[] { new Projeto("Antigo", Base.AddDays(-30)), new Projeto("Novo", Base), new Projeto("Meio", Base.AddDays(-10)) };

        var resultado = Escolher(embaralhada, 2, 1);

        resultado.Primary!.Nome.Should().Be("Novo");
    }

    [Fact]
    public void Pick_DeveDesempatarDatasIguaisPelaOrdemDeEntrada()
    {
        var iguais = new[] { new Projeto("Primeiro", Base), new Projeto("Segundo", Base) };

        Escolher(iguais, 1, 1).Primary!.Nome.Should().Be("Primeiro");
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    public void Pick_DeveDevolverSoOsQueExistem_QuandoHaMenosQueOPedido(int existentes, int menoresEsperados)
    {
        var resultado = Escolher(Lista(existentes), 3, 42);

        resultado.Primary.Should().NotBeNull();
        resultado.Secondary.Should().HaveCount(menoresEsperados);
    }

    [Fact]
    public void Pick_DeveDevolverVazio_QuandoNaoHaDestaques()
    {
        var resultado = Escolher(Array.Empty<Projeto>(), 3, 42);

        resultado.Primary.Should().BeNull();
        resultado.Secondary.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Pick_NaoDeveDevolverMenores_QuandoOPedidoEhZeroOuNegativo(int menores)
    {
        var resultado = Escolher(Lista(5), menores, 42);

        resultado.Primary!.Nome.Should().Be("P0");
        resultado.Secondary.Should().BeEmpty();
    }

    [Fact]
    public void Pick_DeveAceitarSementeZero()
    {
        var resultado = Escolher(Lista(5), 3, 0);

        resultado.Secondary.Should().HaveCount(3).And.OnlyHaveUniqueItems();
    }

    [Fact]
    public void Pick_NaoDeveAlterarAListaDeEntrada()
    {
        var lista = Lista(6);
        var antes = lista.ToList();

        Escolher(lista, 3, 99);

        lista.Should().Equal(antes);
    }

    [Theory]
    [InlineData("12345", 999, 12345, false)]
    [InlineData("-7", 999, -7, false)]
    [InlineData("0", 999, 0, false)]
    [InlineData(null, 999, 999, true)]
    [InlineData("", 999, 999, true)]
    [InlineData("abc", 999, 999, true)]
    [InlineData("12.5", 999, 999, true)]
    [InlineData("99999999999", 999, 999, true)]
    public void ResolveSeed_DeveUsarAGuardadaSeValida_SenaoAReserva(string? guardada, int reserva, int esperada, bool nova)
    {
        var semente = FeaturedSelector.ResolveSeed(guardada, reserva, out var isNew);

        semente.Should().Be(esperada);
        isNew.Should().Be(nova);
    }
}
