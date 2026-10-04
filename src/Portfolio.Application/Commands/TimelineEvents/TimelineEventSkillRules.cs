// ====================================
// Título: TimelineEventSkillRules.cs
// Descrição: Regras compartilhadas das habilidades de um evento da timeline
//            (validação nos dois validators e montagem das entidades nos handlers)
// ====================================

using System.Linq.Expressions;
using FluentValidation;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Commands.TimelineEvents;

public static class TimelineEventSkillRules
{
    public const int MaxSkills = 5;
    public const int MaxNameLength = 40;

    /// <summary>
    /// Nome normalizado: sem espaços nas pontas. Usado na validação de duplicados e ao gravar
    /// </summary>
    public static string NormalizeName(string? name) => (name ?? string.Empty).Trim();

    /// <summary>
    /// Categoria válida: nome de SkillCategory (maiúsculas e minúsculas aceitas). Números não valem
    /// </summary>
    public static bool IsValidCategory(string? category) =>
        !string.IsNullOrWhiteSpace(category)
        && !int.TryParse(category, out _)
        && Enum.TryParse<SkillCategory>(category.Trim(), ignoreCase: true, out var parsed)
        && Enum.IsDefined(parsed);

    /// <summary>
    /// Há dois nomes iguais (depois do Trim, ignorando maiúsculas)?
    /// </summary>
    public static bool HasDuplicateNames(IEnumerable<TimelineEventSkillInputDto?>? skills)
    {
        if (skills == null) return false;

        var names = skills
            .Where(s => s != null)
            .Select(s => NormalizeName(s!.Name))
            .Where(n => n.Length > 0)
            .ToList();

        return names.Count != names.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }

    /// <summary>
    /// Aplica as regras de habilidades a um validator (Create e Update usam as mesmas)
    /// </summary>
    public static void Apply<T>(
        AbstractValidator<T> validator,
        Expression<Func<T, IEnumerable<TimelineEventSkillInputDto>?>> skills)
    {
        validator.RuleFor(skills)
            .Must(list => list == null || list.Count() <= MaxSkills)
                .WithMessage($"Informe no máximo {MaxSkills} habilidades por evento")
            .Must(list => !HasDuplicateNames(list))
                .WithMessage("Não repita habilidades no mesmo evento");

        validator.RuleForEach(skills)
            .NotNull().WithMessage("Habilidade inválida")
            .SetValidator(new TimelineEventSkillInputValidator());
    }

    /// <summary>
    /// Monta as entidades a partir da lista enviada: nomes com Trim, categoria convertida e
    /// Order pela posição na lista. Os Ids ficam vazios para o EF gerar ao inserir
    /// </summary>
    public static List<TimelineEventSkill> BuildSkills(IEnumerable<TimelineEventSkillInputDto>? inputs)
    {
        var result = new List<TimelineEventSkill>();
        if (inputs == null) return result;

        var order = 0;
        foreach (var input in inputs)
        {
            result.Add(new TimelineEventSkill
            {
                Name = NormalizeName(input.Name),
                Category = Enum.Parse<SkillCategory>(input.Category.Trim(), ignoreCase: true),
                Order = order++
            });
        }

        return result;
    }
}

/// <summary>
/// Validador de uma habilidade: nome obrigatório (até 40 caracteres, depois do Trim) e categoria válida
/// </summary>
public class TimelineEventSkillInputValidator : AbstractValidator<TimelineEventSkillInputDto>
{
    public TimelineEventSkillInputValidator()
    {
        RuleFor(x => x.Name)
            .Must(name => TimelineEventSkillRules.NormalizeName(name).Length > 0)
                .WithMessage("O nome da habilidade é obrigatório")
            .Must(name => TimelineEventSkillRules.NormalizeName(name).Length <= TimelineEventSkillRules.MaxNameLength)
                .WithMessage($"O nome da habilidade deve ter no máximo {TimelineEventSkillRules.MaxNameLength} caracteres");

        RuleFor(x => x.Category)
            .Must(TimelineEventSkillRules.IsValidCategory)
                .WithMessage("A categoria da habilidade é inválida (use HardSkill, SoftSkill ou Stack)");
    }
}
