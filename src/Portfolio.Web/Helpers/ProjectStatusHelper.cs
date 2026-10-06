// Título: ProjectStatusHelper.cs
// Descrição: Status do projeto no admin: opções do select e conversão do nome devolvido pela API para o valor do formulário

namespace Portfolio.Web.Helpers;

/// <summary>
/// A API devolve o status como NOME ("InProgress") e recebe como NÚMERO (1). Este helper é a única fonte dessa
/// correspondência no Web: o select do formulário e o carregamento usam a mesma lista, então não podem divergir.
/// Os valores seguem o enum ProjectStatus do domínio (um teste confere).
/// </summary>
public static class ProjectStatusHelper
{
    /// <summary>Valor do formulário para um status que a API devolveu mas o Web não reconhece.</summary>
    public const int Unknown = -1;

    public static IReadOnlyList<(int Value, string Name, string Label)> Options { get; } = new[]
    {
        (0, "Planning", "Planejamento"),
        (1, "InProgress", "Em andamento"),
        (2, "Completed", "Concluído"),
        (3, "Archived", "Arquivado")
    };

    /// <summary>
    /// Converte o nome devolvido pela API no valor numérico, sem diferenciar maiúsculas.
    /// Só aceita nomes: um número ou texto desconhecido devolve false, para o formulário avisar em vez de adivinhar.
    /// </summary>
    public static bool TryParse(string? name, out int value)
    {
        foreach (var option in Options)
        {
            if (string.Equals(option.Name, name?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                value = option.Value;
                return true;
            }
        }

        value = Unknown;
        return false;
    }

    /// <summary>True se o valor corresponde a uma das opções do select.</summary>
    public static bool IsValid(int value) => Options.Any(option => option.Value == value);

    /// <summary>Rótulo em português do valor, ou string vazia se não for um status conhecido.</summary>
    public static string Label(int value) =>
        Options.FirstOrDefault(option => option.Value == value).Label ?? string.Empty;
}
