// Título: TimelineHandlerTests.cs
// Descrição: Testes unitários dos handlers de TimelineEvents (Query e Command)

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.TimelineEvents.CreateTimelineEvent;
using Portfolio.Application.Commands.TimelineEvents.DeleteTimelineEvent;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.TimelineEvents.GetAllTimelineEvents;
using Portfolio.Application.Queries.TimelineEvents.GetTimelineEventById;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.TimelineEvents;

public class GetAllTimelineEventsQueryHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTimelineEventsQueryHandler _handler;

    public GetAllTimelineEventsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITimelineEventRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTimelineEventsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeEventos_QuandoExistiremEventos()
    {
        // Arrange
        var eventos = new List<TimelineEvent>
        {
            new() { Id = Guid.NewGuid(), Title = "Primeiro contato com tecnologia" },
            new() { Id = Guid.NewGuid(), Title = "Primeiro emprego como dev" }
        };

        var dtos = new List<TimelineEventCardDto>
        {
            new() { Id = eventos[0].Id, Title = "Primeiro contato com tecnologia" },
            new() { Id = eventos[1].Id, Title = "Primeiro emprego como dev" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(eventos);
        _mapperMock.Setup(m => m.Map<IEnumerable<TimelineEventCardDto>>(eventos)).Returns(dtos);

        // Act
        var resultado = await _handler.Handle(new GetAllTimelineEventsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Title.Should().Be("Primeiro contato com tecnologia");
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremEventos()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TimelineEvent>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TimelineEventCardDto>>(It.IsAny<IEnumerable<TimelineEvent>>()))
            .Returns(new List<TimelineEventCardDto>());

        // Act
        var resultado = await _handler.Handle(new GetAllTimelineEventsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveChamarGetAllAsync_UmaVez()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TimelineEvent>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<TimelineEventDto>>(It.IsAny<IEnumerable<TimelineEvent>>()))
            .Returns(new List<TimelineEventDto>());

        // Act
        await _handler.Handle(new GetAllTimelineEventsQuery(), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
}

public class GetTimelineEventByIdQueryHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTimelineEventByIdQueryHandler _handler;

    public GetTimelineEventByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITimelineEventRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTimelineEventByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarEvento_QuandoEventoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        var evento = new TimelineEvent { Id = id, Title = "Primeiro emprego como dev" };
        var dto = new TimelineEventDto { Id = id, Title = "Primeiro emprego como dev" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(evento);
        _mapperMock.Setup(m => m.Map<TimelineEventDto>(evento)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetTimelineEventByIdQuery(id), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
        resultado.Title.Should().Be("Primeiro emprego como dev");
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoEventoNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TimelineEvent?)null);
        _mapperMock.Setup(m => m.Map<TimelineEventDto?>(null)).Returns((TimelineEventDto?)null);

        // Act
        var resultado = await _handler.Handle(new GetTimelineEventByIdQuery(id), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }
}

public class DeleteTimelineEventCommandHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock;
    private readonly DeleteTimelineEventCommandHandler _handler;

    public DeleteTimelineEventCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITimelineEventRepository>();
        _handler = new DeleteTimelineEventCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveDeletarEvento_QuandoEventoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(new DeleteTimelineEventCommand(id), CancellationToken.None);

        // Assert
        resultado.Should().Be(MediatR.Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoEventoNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(new DeleteTimelineEventCommand(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

public class CreateTimelineEventCommandHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTimelineEventCommandHandler _handler;

    public CreateTimelineEventCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITimelineEventRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTimelineEventCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarEvento_ERetornarDto()
    {
        // Arrange
        var createDto = new CreateTimelineEventDto { Title = "Novo evento" };
        var command = new CreateTimelineEventCommand(createDto);
        var entity = new TimelineEvent { Id = Guid.NewGuid(), Title = "Novo evento" };
        var dto = new TimelineEventDto { Id = entity.Id, Title = "Novo evento" };

        _mapperMock.Setup(m => m.Map<TimelineEvent>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<TimelineEventDto>(entity)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(entity.Id);
        resultado.Title.Should().Be("Novo evento");
    }

    [Fact]
    public async Task Handle_DeveChamarAddAsync_ESaveChangesAsync()
    {
        // Arrange
        var createDto = new CreateTimelineEventDto { Title = "Teste" };
        var command = new CreateTimelineEventCommand(createDto);
        var entity = new TimelineEvent { Id = Guid.NewGuid(), Title = "Teste" };
        var dto = new TimelineEventDto { Id = entity.Id, Title = "Teste" };

        _mapperMock.Setup(m => m.Map<TimelineEvent>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<TimelineEventDto>(entity)).Returns(dto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}