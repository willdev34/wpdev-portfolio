// Título: TimelineTypeHelper.cs
// Descrição: Rótulos em português do tipo de evento da timeline (a API devolve o nome do enum em inglês)

namespace Portfolio.Web.Helpers;

/// <summary>
/// Fonte única dos rótulos do tipo do evento. A listagem do admin, a página pública e o select
/// do formulário usam esta tabela. Os valores inteiros espelham o enum TimelineEventType da API.
/// </summary>
public static class TimelineTypeHelper
{
    /// <summary>
    /// Todos os tipos na ordem do enum: valor inteiro enviado à API, nome do enum e rótulo.
    /// </summary>
    public static IReadOnlyList<(int Value, string Name, string Label)> Options { get; } = new[]
    {
        (0, "Education", "Educação"),
        (1, "Work", "Trabalho"),
        (2, "Project", "Projeto"),
        (3, "Achievement", "Conquista"),
        (4, "Certification", "Certificação"),
        (5, "Other", "Outro")
    };

    /// <summary>
    /// Devolve o rótulo em português do nome do enum. Valor desconhecido devolve o texto original
    /// e nulo devolve string vazia.
    /// </summary>
    public static string Label(string? type)
    {
        if (type is null)
        {
            return string.Empty;
        }

        foreach (var option in Options)
        {
            if (option.Name == type)
            {
                return option.Label;
            }
        }

        return type;
    }
}
