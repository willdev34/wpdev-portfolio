// Título: SkillNetworkLayout.cs
// Descrição: Posição de repouso das bolhas de habilidade em leque ao redor do ponto do evento da timeline

namespace Portfolio.Web.Helpers;

public enum SkillNetworkSide
{
    Left,
    Right
}

/// <summary>
/// Posição de uma bolha em relação ao ponto do evento. O ângulo é em graus, com 0 apontando para a
/// direita e 90 para baixo (eixo Y da tela). X e Y são o mesmo ponto em pixels, já arredondados.
/// Fase (radianos) e amplitude horizontal (pixels) alimentam a oscilação feita pelo script da rede.
/// </summary>
public readonly record struct SkillBubblePosition(
    double Angle,
    double Distance,
    double X,
    double Y,
    double Phase,
    double Amplitude);

/// <summary>
/// Layout determinístico da rede de habilidades: a mesma entrada sempre devolve a mesma saída, sem Random.
/// O card fica num lado e a rede no lado oposto, então o lado alterna pela posição do evento na lista.
///
/// As bolhas ocupam slots verticais de altura fixa, centrados no ponto do evento, e formam um arco
/// (as pontas ficam mais perto da linha central). O espaçamento é por altura, não por ângulo, e já
/// reserva uma bolha de duas linhas mais a oscilação vertical. Como o card do evento vizinho fica do
/// mesmo lado da rede, a altura mínima do item (<see cref="RequiredHeight"/>) mantém toda a rede,
/// em repouso e em movimento, dentro do próprio item.
/// </summary>
public static class SkillNetworkLayout
{
    /// <summary>Altura reservada por bolha: 44 px de bolha com duas linhas, 14 px de oscilação dos vizinhos e folga.</summary>
    public const double SlotHeight = 62;

    /// <summary>Margem livre em cima e embaixo da rede, dentro do item, para a data e o card do evento vizinho.</summary>
    public const double VerticalMargin = 40;

    /// <summary>Menor distância horizontal do centro de uma bolha até a linha central.</summary>
    public const double MinX = 100;

    /// <summary>Amplitude horizontal da oscilação, em pixels. A vertical é metade, para caber no slot.</summary>
    public const double MinAmplitude = 8;
    public const double MaxAmplitude = 14;

    private const double BaseDistance = 190;

    /// <summary>
    /// Lado do card: eventos de posição par ficam à direita, ímpar à esquerda.
    /// </summary>
    public static SkillNetworkSide CardSide(int eventIndex)
    {
        return eventIndex % 2 == 0 ? SkillNetworkSide.Right : SkillNetworkSide.Left;
    }

    /// <summary>
    /// Lado da rede, sempre o oposto ao do card.
    /// </summary>
    public static SkillNetworkSide NetworkSide(int eventIndex)
    {
        return CardSide(eventIndex) == SkillNetworkSide.Right ? SkillNetworkSide.Left : SkillNetworkSide.Right;
    }

    /// <summary>
    /// Altura mínima do item, em pixels, para a rede caber com margem em cima e embaixo.
    /// </summary>
    public static double RequiredHeight(int skillCount)
    {
        return skillCount <= 0 ? 0 : skillCount * SlotHeight + 2 * VerticalMargin;
    }

    /// <summary>
    /// Devolve uma posição por habilidade, na ordem da lista, centrada no ponto do evento. As posições
    /// ficam no lado da rede, a pelo menos <see cref="MinX"/> da linha central, e dentro de
    /// <see cref="RequiredHeight"/> na vertical.
    /// </summary>
    public static IReadOnlyList<SkillBubblePosition> Compute(int eventIndex, SkillNetworkSide networkSide, int skillCount)
    {
        if (skillCount <= 0)
        {
            return Array.Empty<SkillBubblePosition>();
        }

        var sign = networkSide == SkillNetworkSide.Left ? -1.0 : 1.0;
        var halfSpan = (skillCount - 1) / 2.0 * SlotHeight;
        var radius = Math.Max(BaseDistance, halfSpan + 40);

        var result = new SkillBubblePosition[skillCount];
        for (var i = 0; i < skillCount; i++)
        {
            var y = Math.Round((i - (skillCount - 1) / 2.0) * SlotHeight);
            var x = Math.Max(MinX, Math.Round(Math.Sqrt(radius * radius - y * y)));
            var signedX = sign * x;
            var angle = Math.Round(Math.Atan2(y, signedX) * 180.0 / Math.PI, 1);
            var distance = Math.Round(Math.Sqrt(x * x + y * y));

            // Razão áurea espalha as fases sem Random; o índice do evento desloca cada evento
            var phase = Math.Round(((i * 0.618034 + eventIndex * 0.37) % 1.0) * 2 * Math.PI, 3);
            var amplitude = MinAmplitude + (i * 5 + eventIndex * 3) % (MaxAmplitude - MinAmplitude + 1);

            result[i] = new SkillBubblePosition(angle, distance, signedX, y, phase, amplitude);
        }

        return result;
    }
}
