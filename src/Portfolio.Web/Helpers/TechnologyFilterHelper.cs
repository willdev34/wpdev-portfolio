// Título: TechnologyFilterHelper.cs
// Descrição: Regras do filtro de tecnologias da página de projetos (lista única e casamento exato pela chave canônica)

using Portfolio.Application.Helpers;

namespace Portfolio.Web.Helpers;

/// <summary>
/// Usa o TechnologyNormalizer (compilado por link no Web), então o filtro já agrupa variações como CSS3 e CSS 3
/// mesmo antes de o script de limpeza ser rodado no banco.
/// </summary>
public static class TechnologyFilterHelper
{
    /// <summary>
    /// Lista única de tecnologias para os chips: nomes canônicos, sem duplicados, da mais usada para a menos usada.
    /// A frequência é a quantidade de projetos que usam a tecnologia, contada pela chave canônica (CSS, CSS3 e
    /// CSS 3 são uma só, e um projeto que lista mais de uma grafia conta uma vez). O desempate é alfabético,
    /// sem diferenciar maiúsculas. Quem chama passa só os projetos ativos (a API já devolve só esses).
    /// </summary>
    public static List<string> BuildAvailable(IEnumerable<IEnumerable<string>> technologiesByProject)
    {
        return technologiesByProject
            .SelectMany(TechnologyNormalizer.NormalizeAll)
            .GroupBy(TechnologyNormalizer.Key)
            .Select(group => new { Name = group.First(), Projects = group.Count() })
            .OrderByDescending(item => item.Projects)
            .ThenBy(item => item.Name, StringComparer.InvariantCultureIgnoreCase)
            .Select(item => item.Name)
            .ToList();
    }

    /// <summary>
    /// Casamento exato: o projeto passa se tiver pelo menos uma das tecnologias selecionadas.
    /// "Java" não casa "JavaScript" e "Next.js" não casa "Next.js 16". Sem seleção, todos passam.
    /// </summary>
    public static bool MatchesAny(IEnumerable<string> projectTechnologies, IReadOnlyCollection<string> selected)
    {
        if (selected.Count == 0)
        {
            return true;
        }

        var selectedKeys = selected.Select(name => TechnologyNormalizer.Key(TechnologyNormalizer.Normalize(name))).ToHashSet();

        return projectTechnologies.Any(tech => selectedKeys.Contains(TechnologyNormalizer.Key(TechnologyNormalizer.Normalize(tech))));
    }

    public const string ToggleBaseLabel = "Todas as tecnologias";

    /// <summary>
    /// A quantidade só aparece com o filtro recolhido e ao menos uma tecnologia selecionada: expandido,
    /// os chips selecionados já estão visíveis.
    /// </summary>
    public static bool ShowsSelectedCount(int selectedCount, bool expanded) => !expanded && selectedCount > 0;

    /// <summary>
    /// Rótulo acessível do botão de expandir: "Todas as tecnologias" ou "Todas as tecnologias (N)".
    /// </summary>
    public static string BuildToggleLabel(int selectedCount, bool expanded) =>
        ShowsSelectedCount(selectedCount, expanded) ? $"{ToggleBaseLabel} ({selectedCount})" : ToggleBaseLabel;

    /// <summary>
    /// Resolve o valor de ?tech= para o nome exibido no chip, normalizando antes: links antigos como
    /// ?tech=CSS 3 ou ?tech=html5 continuam funcionando. Devolve null se não houver tecnologia equivalente.
    /// </summary>
    public static string? ResolveFromQuery(string? queryValue, IEnumerable<string> available)
    {
        if (string.IsNullOrWhiteSpace(queryValue))
        {
            return null;
        }

        return available.FirstOrDefault(name => TechnologyNormalizer.AreEquivalent(name, queryValue));
    }
}
