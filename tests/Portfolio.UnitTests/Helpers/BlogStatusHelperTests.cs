// Título: BlogStatusHelperTests.cs
// Descrição: Testes unitários do BlogStatusHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.
//            Usa só datas fixas: a hora atual é passada por parâmetro.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class BlogStatusHelperTests
{
    // "Agora" fixo: 15/10/2026 18:00 UTC (15:00 em Brasília)
    private static readonly DateTime Agora = new(2026, 10, 15, 18, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void GetEffectiveStatus_DeveManterScheduled_QuandoAgendadoNoFuturo()
    {
        // Arrange
        var agendamento = Agora.AddMinutes(1);

        // Act
        var status = BlogStatusHelper.GetEffectiveStatus("Scheduled", agendamento, Agora);

        // Assert
        status.Should().Be("Scheduled");
        BlogStatusHelper.IsOverdueScheduled("Scheduled", agendamento, Agora).Should().BeFalse();
    }

    [Fact]
    public void GetEffectiveStatus_DeveVirarPublished_QuandoAgendadoVencido()
    {
        // Arrange
        var agendamento = Agora.AddMinutes(-1);

        // Act
        var status = BlogStatusHelper.GetEffectiveStatus("Scheduled", agendamento, Agora);

        // Assert
        status.Should().Be("Published");
        BlogStatusHelper.IsOverdueScheduled("Scheduled", agendamento, Agora).Should().BeTrue();
    }

    [Fact]
    public void GetEffectiveStatus_DeveVirarPublished_QuandoAgendadoExatamenteNaHora()
    {
        // Act
        var status = BlogStatusHelper.GetEffectiveStatus("Scheduled", Agora, Agora);

        // Assert
        // Mesma regra da API (ScheduledAt <= agora)
        status.Should().Be("Published");
        BlogStatusHelper.IsOverdueScheduled("Scheduled", Agora, Agora).Should().BeTrue();
    }

    [Fact]
    public void GetEffectiveStatus_DeveManterScheduled_QuandoScheduledAtNulo()
    {
        // Act
        var status = BlogStatusHelper.GetEffectiveStatus("Scheduled", null, Agora);

        // Assert
        status.Should().Be("Scheduled");
        BlogStatusHelper.IsOverdueScheduled("Scheduled", null, Agora).Should().BeFalse();
    }

    [Theory]
    [InlineData("Published")]
    [InlineData("Draft")]
    public void GetEffectiveStatus_DeveManterStatus_QuandoPublishedOuDraft(string original)
    {
        // Arrange
        // Mesmo com um ScheduledAt vencido preenchido, só Scheduled é reavaliado
        var dataVencida = Agora.AddDays(-1);

        // Act
        var status = BlogStatusHelper.GetEffectiveStatus(original, dataVencida, Agora);

        // Assert
        status.Should().Be(original);
        BlogStatusHelper.IsOverdueScheduled(original, dataVencida, Agora).Should().BeFalse();
    }
}
