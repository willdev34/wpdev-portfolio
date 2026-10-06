// Título: TechnologyNormalizer.cs
// Descrição: Normaliza nomes de tecnologia (trim, espaços, nomes canônicos e dedupe sem diferenciar maiúsculas)

using System.Text;

namespace Portfolio.Application.Helpers;

/// <summary>
/// Regra única de nomes de tecnologia. Sem dependências de pacotes, porque o Portfolio.Web
/// compila este mesmo arquivo por link (o Web não referencia o Application).
/// Versões (Next.js 16, Django 4.2, Prisma 7) ficam como estão: só os aliases do dicionário são unificados.
/// </summary>
public static class TechnologyNormalizer
{
    public const int MaxLength = 50;

    // Chave (minúscula, sem espaços) -> nome canônico. Também corrige a grafia de marcas.
    private static readonly Dictionary<string, string> Canonical = new(StringComparer.Ordinal)
    {
        ["css"] = "CSS",
        ["css3"] = "CSS",
        ["html"] = "HTML",
        ["html5"] = "HTML",
        ["javascript"] = "JavaScript",
        ["js"] = "JavaScript",
        ["javascript(vanillajs)"] = "JavaScript",
        ["githubpages"] = "GitHub Pages"
    };

    /// <summary>
    /// Chave de comparação: minúscula e sem espaços. "CSS 3", "css3" e "CSS3" geram a mesma chave.
    /// </summary>
    public static string Key(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            if (!char.IsWhiteSpace(c))
            {
                builder.Append(char.ToLowerInvariant(c));
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Nome canônico: espaços das pontas removidos, espaços internos colapsados em um e,
    /// se o nome estiver no dicionário, o nome canônico. Texto vazio vira string vazia.
    /// </summary>
    public static string Normalize(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var cleaned = string.Join(' ', name.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

        return Canonical.TryGetValue(Key(cleaned), out var canonical) ? canonical : cleaned;
    }

    /// <summary>
    /// Normaliza a lista inteira: separa itens com vírgula (o armazenamento usa vírgula como separador),
    /// descarta vazios e remove duplicados sem diferenciar maiúsculas, mantendo a primeira ocorrência e a ordem.
    /// </summary>
    public static List<string> NormalizeAll(IEnumerable<string?>? names)
    {
        var result = new List<string>();
        if (names is null)
        {
            return result;
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in names)
        {
            foreach (var part in (entry ?? string.Empty).Split(','))
            {
                var normalized = Normalize(part);
                if (normalized.Length > 0 && seen.Add(Key(normalized)))
                {
                    result.Add(normalized);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// True quando os dois nomes são a mesma tecnologia depois de normalizados (casamento exato pela chave canônica).
    /// </summary>
    public static bool AreEquivalent(string? a, string? b)
    {
        var keyA = Key(Normalize(a));
        return keyA.Length > 0 && keyA == Key(Normalize(b));
    }
}
