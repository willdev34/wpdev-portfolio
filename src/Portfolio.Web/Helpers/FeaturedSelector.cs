// Título: FeaturedSelector.cs
// Descrição: Escolhe os projetos em destaque da Home: o principal é fixo e os menores são sorteados com semente

namespace Portfolio.Web.Helpers;

/// <summary>Resultado da escolha: o principal (ou null se não há destaques) e os menores já sorteados.</summary>
public sealed record FeaturedSelection<T>(T? Primary, IReadOnlyList<T> Secondary) where T : class;

/// <summary>
/// Regra pura da seção de destaques. O principal é o item de CreatedAt mais recente e nunca entra no sorteio.
/// Os demais passam por um Fisher-Yates com semente: a mesma semente e a mesma lista dão sempre o mesmo resultado.
/// Usa um gerador próprio (xorshift32) em vez de System.Random, para a ordem não mudar entre versões do .NET.
/// </summary>
public static class FeaturedSelector
{
    /// <summary>
    /// Escolhe o principal e até secondaryCount menores. Com menos destaques que o pedido, devolve só os que existem.
    /// A ordem de entrada só desempata datas iguais (a primeira vence).
    /// </summary>
    public static FeaturedSelection<T> Pick<T>(
        IEnumerable<T> featured, Func<T, DateTime> createdAt, int secondaryCount, int seed) where T : class
    {
        // OrderByDescending é estável: datas iguais mantêm a ordem em que chegaram
        var ordered = featured.OrderByDescending(createdAt).ToList();

        if (ordered.Count == 0)
        {
            return new FeaturedSelection<T>(null, Array.Empty<T>());
        }

        var primary = ordered[0];
        var rest = ordered.Skip(1).ToList();

        Shuffle(rest, seed);

        return new FeaturedSelection<T>(primary, rest.Take(Math.Max(0, secondaryCount)).ToList());
    }

    /// <summary>
    /// Usa a semente guardada na sessão quando ela é um inteiro válido; senão usa a de reserva.
    /// isNew diz se a de reserva foi usada, para quem chama gravá-la na sessão.
    /// </summary>
    public static int ResolveSeed(string? stored, int fallbackSeed, out bool isNew)
    {
        if (int.TryParse(stored, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
        {
            isNew = false;
            return parsed;
        }

        isNew = true;
        return fallbackSeed;
    }

    private static void Shuffle<T>(List<T> items, int seed)
    {
        // xorshift32 não aceita estado zero
        var state = seed == 0 ? 0x9E3779B9u : (uint)seed;

        for (var i = items.Count - 1; i > 0; i--)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;

            var j = (int)(state % (uint)(i + 1));
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
