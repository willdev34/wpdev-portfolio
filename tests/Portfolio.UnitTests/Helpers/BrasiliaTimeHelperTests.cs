// Título: BrasiliaTimeHelperTests.cs
// Descrição: Testes unitários do BrasiliaTimeHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.
//            Usa só datas fixas: o offset de Brasília é fixo em -3 horas.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class BrasiliaTimeHelperTests
{
    [Fact]
    public void ToUtc_DeveSomarTresHoras_ERetornarKindUtc()
    {
        // Arrange
        var brasilia = new DateTime(2026, 10, 15, 15, 0, 0, DateTimeKind.Unspecified);

        // Act
        var utc = BrasiliaTimeHelper.ToUtc(brasilia);

        // Assert
        utc.Should().Be(new DateTime(2026, 10, 15, 18, 0, 0));
        utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ToUtc_DeveVirarODia_QuandoHorarioDepoisDas21h()
    {
        // Arrange
        var brasilia = new DateTime(2026, 12, 31, 22, 0, 0, DateTimeKind.Unspecified);

        // Act
        var utc = BrasiliaTimeHelper.ToUtc(brasilia);

        // Assert
        utc.Should().Be(new DateTime(2027, 1, 1, 1, 0, 0));
        utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ToUtc_DeveTratarComoBrasilia_QuandoKindLocal()
    {
        // Arrange
        // O valor do input é sempre horário de Brasília, independente do Kind que chegar
        var brasilia = new DateTime(2026, 10, 15, 15, 0, 0, DateTimeKind.Local);

        // Act
        var utc = BrasiliaTimeHelper.ToUtc(brasilia);

        // Assert
        utc.Should().Be(new DateTime(2026, 10, 15, 18, 0, 0));
        utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUtc_DeveSubtrairTresHoras_QuandoKindUtc()
    {
        // Arrange
        var utc = new DateTime(2026, 10, 15, 18, 0, 0, DateTimeKind.Utc);

        // Act
        var brasilia = BrasiliaTimeHelper.FromUtc(utc);

        // Assert
        brasilia.Should().Be(new DateTime(2026, 10, 15, 15, 0, 0));
        brasilia.Kind.Should().Be(DateTimeKind.Unspecified);
    }

    [Fact]
    public void FromUtc_DeveTratarComoUtc_QuandoKindUnspecified()
    {
        // Arrange
        // API sem sufixo "Z" no JSON gera DateTime Unspecified; o helper assume UTC
        var utc = new DateTime(2026, 10, 15, 18, 0, 0, DateTimeKind.Unspecified);

        // Act
        var brasilia = BrasiliaTimeHelper.FromUtc(utc);

        // Assert
        brasilia.Should().Be(new DateTime(2026, 10, 15, 15, 0, 0));
        brasilia.Kind.Should().Be(DateTimeKind.Unspecified);
    }

    [Fact]
    public void FromUtc_DeveVoltarODia_QuandoUtcAntesDas3h()
    {
        // Arrange
        var utc = new DateTime(2027, 1, 1, 1, 0, 0, DateTimeKind.Utc);

        // Act
        var brasilia = BrasiliaTimeHelper.FromUtc(utc);

        // Assert
        brasilia.Should().Be(new DateTime(2026, 12, 31, 22, 0, 0));
    }

    [Theory]
    [InlineData("2026-10-05T15:00")]
    [InlineData("2026-10-05T15:00:00")]
    [InlineData("2026-10-05T15:00:00.000")]
    public void TryParseInput_DeveAceitar_QuandoFormatoDoDatetimeLocal(string valor)
    {
        // Act
        var ok = BrasiliaTimeHelper.TryParseInput(valor, out var brasilia);

        // Assert
        ok.Should().BeTrue();
        brasilia.Should().Be(new DateTime(2026, 10, 5, 15, 0, 0));
        brasilia.Kind.Should().Be(DateTimeKind.Unspecified);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("05/10/2026 15:00")]
    public void TryParseInput_DeveRejeitar_QuandoVazioNuloOuFormatoDeExibicao(string? valor)
    {
        // Act
        var ok = BrasiliaTimeHelper.TryParseInput(valor, out _);

        // Assert
        ok.Should().BeFalse();
    }

    [Fact]
    public void TryParseInput_SeguidoDeToUtc_DeveTransformar15hEm18hUtc()
    {
        // Arrange
        BrasiliaTimeHelper.TryParseInput("2026-10-05T15:00", out var brasilia).Should().BeTrue();

        // Act
        var utc = BrasiliaTimeHelper.ToUtc(brasilia);

        // Assert
        utc.Should().Be(new DateTime(2026, 10, 5, 18, 0, 0));
        utc.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ToUtc_EFromUtc_DevemSerInversos()
    {
        // Arrange
        var brasilia = new DateTime(2026, 10, 15, 15, 30, 0, DateTimeKind.Unspecified);

        // Act
        var idaEVolta = BrasiliaTimeHelper.FromUtc(BrasiliaTimeHelper.ToUtc(brasilia));

        // Assert
        idaEVolta.Should().Be(brasilia);
    }
}
