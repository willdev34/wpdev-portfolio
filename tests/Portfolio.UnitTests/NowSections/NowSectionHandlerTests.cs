// Título: NowSectionAndProjectHandlerTests.cs
// Descrição: Testes unitários dos handlers de NowSection (Query) e DeleteProject (Command)

using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Portfolio.Application.Commands.Projects.CreateProject;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.NowSections.GetActiveNowSection;
using Portfolio.Application.Queries.NowSections.GetAllNowSections;
using Portfolio.Application.Queries.Projects.GetAllProjects;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.NowSections;

public class GetActiveNowSectionQueryHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetActiveNowSectionQueryHandler _handler;

    public GetActiveNowSectionQueryHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetActiveNowSectionQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSecaoAtiva_QuandoExistir()
    {
        // Arrange
        var secao = new NowSection { Id = Guid.NewGuid(), IsActive = true };
        var dto = new NowSectionDto { Id = secao.Id };

        _repositoryMock.Setup(r => r.GetActiveAsync()).ReturnsAsync(secao);
        _mapperMock.Setup(m => m.Map<NowSectionDto>(secao)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetActiveNowSectionQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(secao.Id);
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoNaoExistirSecaoAtiva()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetActiveAsync()).ReturnsAsync((NowSection?)null);

        // Act
        var resultado = await _handler.Handle(new GetActiveNowSectionQuery(), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        _mapperMock.Verify(m => m.Map<NowSectionDto>(It.IsAny<NowSection>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NaoDeveChamarMapper_QuandoSecaoNaoExistir()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetActiveAsync()).ReturnsAsync((NowSection?)null);

        // Act
        await _handler.Handle(new GetActiveNowSectionQuery(), CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<NowSectionDto>(It.IsAny<object>()), Times.Never);
    }
}

public class GetAllNowSectionsQueryHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllNowSectionsQueryHandler _handler;

    public GetAllNowSectionsQueryHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllNowSectionsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeSecoes_QuandoExistiremSecoes()
    {
        // Arrange
        var secoes = new List<NowSection>
        {
            new() { Id = Guid.NewGuid(), IsActive = true },
            new() { Id = Guid.NewGuid(), IsActive = false }
        };

        var dtos = new List<NowSectionDto>
        {
            new() { Id = secoes[0].Id },
            new() { Id = secoes[1].Id }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(secoes);
        _mapperMock.Setup(m => m.Map<IEnumerable<NowSectionDto>>(secoes)).Returns(dtos);

        // Act
        var resultado = await _handler.Handle(new GetAllNowSectionsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremSecoes()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<NowSection>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<NowSectionDto>>(It.IsAny<IEnumerable<NowSection>>()))
            .Returns(new List<NowSectionDto>());

        // Act
        var resultado = await _handler.Handle(new GetAllNowSectionsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().BeEmpty();
    }
}