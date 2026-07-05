// Título: ProjectHandlerTests.cs
// Descrição: Testes unitários dos handlers de Projects (Query e Command)

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.Projects.CreateProject;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.Projects.GetAllProjects;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.Projects;

public class GetAllProjectsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllProjectsQueryHandler _handler;

    public GetAllProjectsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllProjectsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeProjetos_QuandoExistiremProjetos()
    {
        // Arrange
        var projetos = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "DrenaMais", Description = "Site institucional" },
            new() { Id = Guid.NewGuid(), Title = "IOGAR", Description = "Sistema de precificação" }
        };

        var dtos = new List<ProjectCardDto>
        {
            new() { Id = projetos[0].Id, Title = "DrenaMais" },
            new() { Id = projetos[1].Id, Title = "IOGAR" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(projetos);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProjectCardDto>>(projetos)).Returns(dtos);

        // Act
        var resultado = await _handler.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Title.Should().Be("DrenaMais");
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremProjetos()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Project>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<ProjectCardDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(new List<ProjectCardDto>());

        // Act
        var resultado = await _handler.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveChamarGetAllAsync_UmaVez()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Project>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<ProjectCardDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(new List<ProjectCardDto>());

        // Act
        await _handler.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
}

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProjectCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarProjeto_ERetornarDto()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Title = "Novo Projeto",
            Description = "Descrição do projeto"
        };

        var command = new CreateProjectCommand(createDto);
        var entity = new Project { Id = Guid.NewGuid(), Title = "Novo Projeto" };
        var dto = new ProjectDto { Id = entity.Id, Title = "Novo Projeto" };

        _mapperMock.Setup(m => m.Map<Project>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<ProjectDto>(entity)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(entity.Id);
        resultado.Title.Should().Be("Novo Projeto");
    }

    [Fact]
    public async Task Handle_DeveChamarAddAsync_ESaveChangesAsync()
    {
        // Arrange
        var createDto = new CreateProjectDto { Title = "Teste", Description = "Desc" };
        var command = new CreateProjectCommand(createDto);
        var entity = new Project { Id = Guid.NewGuid(), Title = "Teste" };
        var dto = new ProjectDto { Id = entity.Id, Title = "Teste" };

        _mapperMock.Setup(m => m.Map<Project>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<ProjectDto>(entity)).Returns(dto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}