// Título: TechnologyCleanupPlanner.cs
// Descrição: Calcula, sem tocar no banco, quais projetos têm nomes de tecnologia a corrigir

using Portfolio.Application.Helpers;

namespace DatabaseSetup;

/// <summary>Um projeto com as tecnologias como estão hoje.</summary>
public sealed record ProjectTechnologies(Guid Id, string Title, IReadOnlyList<string> Technologies);

/// <summary>Um projeto cujas tecnologias mudam, com o antes e o depois.</summary>
public sealed record TechnologyChange(Guid Id, string Title, IReadOnlyList<string> Before, IReadOnlyList<string> After);

public static class TechnologyCleanupPlanner
{
    /// <summary>
    /// Devolve só os projetos que mudam ao aplicar o TechnologyNormalizer. Projetos já normalizados
    /// não aparecem, então rodar o plano de novo depois de gravar devolve lista vazia (idempotente).
    /// </summary>
    public static List<TechnologyChange> Plan(IEnumerable<ProjectTechnologies> projects)
    {
        var changes = new List<TechnologyChange>();

        foreach (var project in projects)
        {
            var after = TechnologyNormalizer.NormalizeAll(project.Technologies);

            if (!project.Technologies.SequenceEqual(after, StringComparer.Ordinal))
            {
                changes.Add(new TechnologyChange(project.Id, project.Title, project.Technologies, after));
            }
        }

        return changes;
    }
}
