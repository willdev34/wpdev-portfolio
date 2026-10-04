// Título: TimelineEventSkillsHandlerTests.cs
// Descrição: Testes unitários dos handlers de Create e Update de TimelineEvent no que toca às habilidades
//            (normalização, Order pela posição, nulo mantém, lista vazia limpa, lista substitui)
//            e do mapeamento (leitura ordenada por Order, Skills ignorado em Create e Update)

using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;
using Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Mappings;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.UnitTests.TimelineEvents;

public class CreateTimelineEventSkillsHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly CreateTimelineEventCommandHandler _handler;

    public CreateTimelineEventSkillsHandlerTests()
    {
        _handler = new CreateTimelineEventCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveMontarHabilidadesComTrimCategoriaEOrderPelaPosicao()
    {
        var createDto = new CreateTimelineEventDto
        {
            Title = "Evento",
            Skills = new List<TimelineEventSkillInputDto>
            {
                new() { Name = "  C#  ", Category = "stack" },
                new() { Name = "Comunicação", Category = "SoftSkill" },
                new() { Name = "SQL", Category = "HardSkill" }
            }
        };
        var entity = new TimelineEvent { Id = Guid.NewGuid(), Title = "Evento" };
        List<TimelineEventSkill>? skillsAtAdd = null;

        _mapperMock.Setup(m => m.Map<TimelineEvent>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity))
            .Callback<TimelineEvent>(e => skillsAtAdd = e.Skills.ToList())
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TimelineEventDto>(entity)).Returns(new TimelineEventDto());

        await _handler.Handle(new CreateTimelineEventCommand(createDto), CancellationToken.None);

        skillsAtAdd.Should().NotBeNull();
        skillsAtAdd!.Select(s => s.Name).Should().Equal("C#", "Comunicação", "SQL");
        skillsAtAdd.Select(s => s.Category).Should().Equal(SkillCategory.Stack, SkillCategory.SoftSkill, SkillCategory.HardSkill);
        skillsAtAdd.Select(s => s.Order).Should().Equal(0, 1, 2);
        // Os Ids ficam vazios: o EF gera ao inserir
        skillsAtAdd.Should().OnlyContain(s => s.Id == Guid.Empty);
    }

    [Fact]
    public async Task Handle_NaoDeveTerHabilidades_QuandoListaVazia()
    {
        var createDto = new CreateTimelineEventDto { Title = "Evento" };
        var entity = new TimelineEvent { Id = Guid.NewGuid(), Title = "Evento" };

        _mapperMock.Setup(m => m.Map<TimelineEvent>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TimelineEventDto>(entity)).Returns(new TimelineEventDto());

        await _handler.Handle(new CreateTimelineEventCommand(createDto), CancellationToken.None);

        entity.Skills.Should().BeEmpty();
    }
}

public class UpdateTimelineEventSkillsHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UpdateTimelineEventCommandHandler _handler;

    public UpdateTimelineEventSkillsHandlerTests()
    {
        _handler = new UpdateTimelineEventCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    private (UpdateTimelineEventDto Dto, TimelineEvent Existing) Arrange(List<TimelineEventSkillInputDto>? skills)
    {
        var existing = new TimelineEvent { Id = Guid.NewGuid(), Title = "Evento" };
        var dto = new UpdateTimelineEventDto { Id = existing.Id, Title = "Evento editado", Skills = skills };

        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map(dto, existing)).Returns(existing);

        return (dto, existing);
    }

    [Fact]
    public async Task Handle_NaoDeveMexerNasHabilidades_QuandoSkillsNulo()
    {
        var (dto, _) = Arrange(null);

        await _handler.Handle(new UpdateTimelineEventCommand(dto), CancellationToken.None);

        _repositoryMock.Verify(r => r.ReplaceSkillsAsync(It.IsAny<TimelineEvent>(), It.IsAny<IEnumerable<TimelineEventSkill>>()), Times.Never);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TimelineEvent>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLimparHabilidades_QuandoListaVazia()
    {
        var (dto, existing) = Arrange(new List<TimelineEventSkillInputDto>());
        IEnumerable<TimelineEventSkill>? received = null;
        _repositoryMock.Setup(r => r.ReplaceSkillsAsync(existing, It.IsAny<IEnumerable<TimelineEventSkill>>()))
            .Callback<TimelineEvent, IEnumerable<TimelineEventSkill>>((_, s) => received = s.ToList())
            .Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateTimelineEventCommand(dto), CancellationToken.None);

        received.Should().NotBeNull();
        received.Should().BeEmpty();
        _repositoryMock.Verify(r => r.ReplaceSkillsAsync(existing, It.IsAny<IEnumerable<TimelineEventSkill>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveSubstituirHabilidades_QuandoListaComItens()
    {
        var (dto, existing) = Arrange(new List<TimelineEventSkillInputDto>
        {
            new() { Name = " Go ", Category = "Stack" },
            new() { Name = "Liderança", Category = "softskill" }
        });
        List<TimelineEventSkill>? received = null;
        _repositoryMock.Setup(r => r.ReplaceSkillsAsync(existing, It.IsAny<IEnumerable<TimelineEventSkill>>()))
            .Callback<TimelineEvent, IEnumerable<TimelineEventSkill>>((_, s) => received = s.ToList())
            .Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateTimelineEventCommand(dto), CancellationToken.None);

        received.Should().NotBeNull();
        received!.Select(s => s.Name).Should().Equal("Go", "Liderança");
        received.Select(s => s.Category).Should().Equal(SkillCategory.Stack, SkillCategory.SoftSkill);
        received.Select(s => s.Order).Should().Equal(0, 1);
    }

    [Fact]
    public async Task Handle_DeveSubstituirHabilidadesAntesDoUnicoSaveChanges()
    {
        // Evento e habilidades vão na mesma transação: um único SaveChanges, depois do ReplaceSkills
        var (dto, existing) = Arrange(new List<TimelineEventSkillInputDto> { new() { Name = "Go", Category = "Stack" } });
        var calls = new List<string>();
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Callback(() => calls.Add("Update")).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.ReplaceSkillsAsync(existing, It.IsAny<IEnumerable<TimelineEventSkill>>()))
            .Callback(() => calls.Add("ReplaceSkills")).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).Callback(() => calls.Add("SaveChanges")).ReturnsAsync(1);

        await _handler.Handle(new UpdateTimelineEventCommand(dto), CancellationToken.None);

        calls.Should().Equal("Update", "ReplaceSkills", "SaveChanges");
    }

    [Fact]
    public async Task Handle_DeveLancarKeyNotFound_QuandoEventoNaoExiste()
    {
        var dto = new UpdateTimelineEventDto { Id = Guid.NewGuid(), Skills = new() };
        _repositoryMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((TimelineEvent?)null);

        var act = async () => await _handler.Handle(new UpdateTimelineEventCommand(dto), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _repositoryMock.Verify(r => r.ReplaceSkillsAsync(It.IsAny<TimelineEvent>(), It.IsAny<IEnumerable<TimelineEventSkill>>()), Times.Never);
    }
}

public class TimelineEventSkillsMappingTests
{
    private readonly IMapper _mapper = new MapperConfiguration(
        cfg => cfg.AddProfile<TimelineEventMappingProfile>(),
        NullLoggerFactory.Instance).CreateMapper();

    private static TimelineEvent BuildEventWithSkillsOutOfOrder() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Evento",
        Skills = new List<TimelineEventSkill>
        {
            new() { Id = Guid.NewGuid(), Name = "Terceira", Category = SkillCategory.Stack, Order = 2 },
            new() { Id = Guid.NewGuid(), Name = "Primeira", Category = SkillCategory.HardSkill, Order = 0 },
            new() { Id = Guid.NewGuid(), Name = "Segunda", Category = SkillCategory.SoftSkill, Order = 1 }
        }
    };

    [Fact]
    public void Map_DeveOrdenarHabilidadesPorOrder_NoTimelineEventDto()
    {
        var dto = _mapper.Map<TimelineEventDto>(BuildEventWithSkillsOutOfOrder());

        dto.Skills.Select(s => s.Name).Should().Equal("Primeira", "Segunda", "Terceira");
        dto.Skills.Select(s => s.Category).Should().Equal("HardSkill", "SoftSkill", "Stack");
    }

    [Fact]
    public void Map_DeveOrdenarHabilidadesPorOrder_NoTimelineEventCardDto()
    {
        var dto = _mapper.Map<TimelineEventCardDto>(BuildEventWithSkillsOutOfOrder());

        dto.Skills.Select(s => s.Name).Should().Equal("Primeira", "Segunda", "Terceira");
    }

    [Fact]
    public void Map_DeveIgnorarSkills_NoCreate()
    {
        var createDto = new CreateTimelineEventDto
        {
            Title = "Evento",
            Skills = new() { new() { Name = "C#", Category = "Stack" } }
        };

        var entity = _mapper.Map<TimelineEvent>(createDto);

        entity.Skills.Should().BeEmpty();
    }

    [Fact]
    public void Map_DeveIgnorarSkills_NoUpdate()
    {
        var existing = BuildEventWithSkillsOutOfOrder();
        var updateDto = new UpdateTimelineEventDto
        {
            Id = existing.Id,
            Title = "Editado",
            Skills = new() { new() { Name = "Outra", Category = "Stack" } }
        };

        _mapper.Map(updateDto, existing);

        existing.Title.Should().Be("Editado");
        // O mapper não toca nas habilidades: quem cuida delas é o handler
        existing.Skills.Select(s => s.Name).Should().BeEquivalentTo("Terceira", "Primeira", "Segunda");
    }
}
