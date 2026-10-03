// Título: TimelineEventSkillsValidatorTests.cs
// Descrição: Testes unitários das regras de habilidades dos validators de Create e Update de TimelineEvent
//            (limite de 5, nome obrigatório até 40 caracteres, duplicados com Trim e sem diferenciar
//            maiúsculas, categoria válida e nulo aceito no Update)

using FluentValidation.TestHelper;
using Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;
using Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;
using Portfolio.Application.DTOs.TimelineEvents;

namespace Portfolio.UnitTests.TimelineEvents;

public class CreateTimelineEventSkillsValidatorTests
{
    private readonly CreateTimelineEventCommandValidator _validator = new();

    private static CreateTimelineEventDto BuildValidDto(params TimelineEventSkillInputDto[] skills) => new()
    {
        Title = "Evento válido",
        Description = "Descrição válida com mais de dez caracteres.",
        Date = DateTime.UtcNow.Date,
        Type = 1,
        Skills = skills.ToList()
    };

    private static TimelineEventSkillInputDto Skill(string name, string category = "Stack") =>
        new() { Name = name, Category = category };

    [Fact]
    public void Validate_DevePassar_QuandoNaoHaHabilidades()
    {
        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(BuildValidDto()));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoExatamenteCincoHabilidades()
    {
        var dto = BuildValidDto(Enumerable.Range(1, 5).Select(i => Skill($"Skill {i}")).ToArray());

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoMaisDeCincoHabilidades()
    {
        var dto = BuildValidDto(Enumerable.Range(1, 6).Select(i => Skill($"Skill {i}")).ToArray());

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor(x => x.EventData.Skills)
            .WithErrorMessage("Informe no máximo 5 habilidades por evento");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_DeveFalhar_QuandoNomeVazioOuSoEspacos(string name)
    {
        var dto = BuildValidDto(Skill(name));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Name")
            .WithErrorMessage("O nome da habilidade é obrigatório");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoNomeTemQuarentaEUmCaracteres()
    {
        var dto = BuildValidDto(Skill(new string('a', 41)));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Name")
            .WithErrorMessage("O nome da habilidade deve ter no máximo 40 caracteres");
    }

    [Fact]
    public void Validate_DevePassar_QuandoNomeTemQuarentaCaracteresMaisEspacosNasPontas()
    {
        // O limite vale para o nome depois do Trim
        var dto = BuildValidDto(Skill("  " + new string('a', 40) + "  "));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("C#", "C#")]
    [InlineData("C#", "c#")]
    [InlineData("Docker", " DOCKER ")]
    [InlineData("  sql", "SQL  ")]
    public void Validate_DeveFalhar_QuandoNomesRepetidosComTrimEIgnorandoMaiusculas(string first, string second)
    {
        var dto = BuildValidDto(Skill(first), Skill("Outra"), Skill(second));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor(x => x.EventData.Skills)
            .WithErrorMessage("Não repita habilidades no mesmo evento");
    }

    [Fact]
    public void Validate_DevePassar_QuandoNomesDiferentesNaCategoriaIgual()
    {
        var dto = BuildValidDto(Skill("C#"), Skill("C++"), Skill("C"));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Banana")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("0")]   // número não vale, só o nome do enum
    [InlineData("1")]
    [InlineData("99")]
    public void Validate_DeveFalhar_QuandoCategoriaInvalidaOuNumerica(string category)
    {
        var dto = BuildValidDto(Skill("C#", category));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Category")
            .WithErrorMessage("A categoria da habilidade é inválida (use HardSkill, SoftSkill ou Stack)");
    }

    [Theory]
    [InlineData("HardSkill")]
    [InlineData("SoftSkill")]
    [InlineData("Stack")]
    [InlineData("stack")]
    [InlineData("SOFTSKILL")]
    public void Validate_DevePassar_QuandoCategoriaValida(string category)
    {
        var dto = BuildValidDto(Skill("C#", category));

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoHabilidadeNula()
    {
        var dto = BuildValidDto();
        dto.Skills = new List<TimelineEventSkillInputDto> { null! };

        var resultado = _validator.TestValidate(new CreateTimelineEventCommand(dto));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0]")
            .WithErrorMessage("Habilidade inválida");
    }
}

public class UpdateTimelineEventSkillsValidatorTests
{
    private readonly UpdateTimelineEventCommandValidator _validator = new();

    private static UpdateTimelineEventDto BuildValidDto(List<TimelineEventSkillInputDto>? skills) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Evento válido",
        Description = "Descrição válida com mais de dez caracteres.",
        Date = DateTime.UtcNow.Date,
        Type = 1,
        IsVisible = true,
        Skills = skills
    };

    private static TimelineEventSkillInputDto Skill(string name, string category = "Stack") =>
        new() { Name = name, Category = category };

    [Fact]
    public void Validate_DevePassar_QuandoSkillsNulo()
    {
        // Nulo (campo omitido) significa "manter as habilidades atuais"
        var resultado = _validator.TestValidate(new UpdateTimelineEventCommand(BuildValidDto(null)));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoListaVazia()
    {
        // Lista vazia significa "limpar as habilidades"
        var resultado = _validator.TestValidate(new UpdateTimelineEventCommand(BuildValidDto(new())));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DevePassar_QuandoExatamenteCincoHabilidades()
    {
        var skills = Enumerable.Range(1, 5).Select(i => Skill($"Skill {i}")).ToList();

        var resultado = _validator.TestValidate(new UpdateTimelineEventCommand(BuildValidDto(skills)));

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoMaisDeCincoHabilidades()
    {
        var skills = Enumerable.Range(1, 6).Select(i => Skill($"Skill {i}")).ToList();

        var resultado = _validator.TestValidate(new UpdateTimelineEventCommand(BuildValidDto(skills)));

        resultado.ShouldHaveValidationErrorFor(x => x.EventData.Skills)
            .WithErrorMessage("Informe no máximo 5 habilidades por evento");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoNomeVazio()
    {
        var resultado = _validator.TestValidate(new UpdateTimelineEventCommand(BuildValidDto(new() { Skill("  ") })));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Name")
            .WithErrorMessage("O nome da habilidade é obrigatório");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoNomeTemQuarentaEUmCaracteres()
    {
        var resultado = _validator.TestValidate(
            new UpdateTimelineEventCommand(BuildValidDto(new() { Skill(new string('a', 41)) })));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Name");
    }

    [Fact]
    public void Validate_DeveFalhar_QuandoNomesRepetidosComTrimEIgnorandoMaiusculas()
    {
        var resultado = _validator.TestValidate(
            new UpdateTimelineEventCommand(BuildValidDto(new() { Skill("Go"), Skill(" gO ") })));

        resultado.ShouldHaveValidationErrorFor(x => x.EventData.Skills)
            .WithErrorMessage("Não repita habilidades no mesmo evento");
    }

    [Theory]
    [InlineData("Banana")]
    [InlineData("1")]
    public void Validate_DeveFalhar_QuandoCategoriaInvalidaOuNumerica(string category)
    {
        var resultado = _validator.TestValidate(
            new UpdateTimelineEventCommand(BuildValidDto(new() { Skill("Go", category) })));

        resultado.ShouldHaveValidationErrorFor("EventData.Skills[0].Category");
    }
}
