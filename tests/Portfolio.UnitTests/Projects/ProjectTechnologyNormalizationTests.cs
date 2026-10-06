// Título: ProjectTechnologyNormalizationTests.cs
// Descrição: Garante que criar e atualizar projeto gravam tecnologias normalizadas e que os validators aceitam/rejeitam os casos esperados

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.Projects.CreateProject;
using Portfolio.Application.Commands.Projects.UpdateProject;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.Projects;

public class ProjectTechnologyNormalizationTests
{
    private readonly Mock<IProjectRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task CreateHandler_DeveNormalizarAsTecnologias_AntesDeMapear()
    {
        // Arrange
        var dto = new CreateProjectDto { Title = "Site", Technologies = new() { "Javascript", "CSS 3", "css", "HTML5", "React" } };
        var entity = new Project { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<Project>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ProjectDto>(entity)).Returns(new ProjectDto());
        var handler = new CreateProjectCommandHandler(_repositoryMock.Object, _mapperMock.Object);

        // Act
        await handler.Handle(new CreateProjectCommand(dto), CancellationToken.None);

        // Assert
        dto.Technologies.Should().Equal("JavaScript", "CSS", "HTML", "React");
    }

    [Fact]
    public async Task UpdateHandler_DeveNormalizarAsTecnologias_AntesDeMapear()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProjectDto { Id = id, Title = "Site", Technologies = new() { "Github Pages", "JS", "javascript", "HTML 5" } };
        var existing = new Project { Id = id };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map(dto, existing)).Returns(existing);
        var handler = new UpdateProjectCommandHandler(_repositoryMock.Object, _mapperMock.Object);

        // Act
        await handler.Handle(new UpdateProjectCommand(dto), CancellationToken.None);

        // Assert
        dto.Technologies.Should().Equal("GitHub Pages", "JavaScript", "HTML");
    }

    [Fact]
    public void CreateValidator_DeveRejeitar_QuandoSoHaTecnologiasEmBranco()
    {
        var dto = DtoValidoComTecnologias(" ", "");

        var resultado = new CreateProjectCommandValidator().Validate(new CreateProjectCommand(dto));

        resultado.Errors.Should().Contain(e => e.PropertyName.Contains("Technologies"));
    }

    [Fact]
    public void CreateValidator_DeveContarTecnologiasDepoisDeNormalizar()
    {
        // 12 entradas, mas só 1 tecnologia distinta depois de normalizar: não passa de 10
        var entradas = Enumerable.Range(0, 4).SelectMany(_ => new[] { "CSS", "CSS3", "CSS 3" }).ToArray();
        var dto = DtoValidoComTecnologias(entradas);

        var resultado = new CreateProjectCommandValidator().Validate(new CreateProjectCommand(dto));

        resultado.Errors.Should().NotContain(e => e.PropertyName.Contains("Technologies"));
    }

    [Fact]
    public void CreateValidator_DeveRejeitar_QuandoHaMaisDeDezTecnologiasDistintas()
    {
        var dto = DtoValidoComTecnologias(Enumerable.Range(1, 11).Select(i => $"Tech{i}").ToArray());

        var resultado = new CreateProjectCommandValidator().Validate(new CreateProjectCommand(dto));

        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("no máximo 10"));
    }

    [Fact]
    public void CreateValidator_DeveRejeitar_QuandoUmNomeTemMaisDe50Caracteres()
    {
        var dto = DtoValidoComTecnologias(new string('a', 51));

        var resultado = new CreateProjectCommandValidator().Validate(new CreateProjectCommand(dto));

        resultado.Errors.Should().Contain(e => e.ErrorMessage.Contains("50 caracteres"));
    }

    [Fact]
    public void UpdateValidator_DeveRejeitar_QuandoSoHaTecnologiasEmBranco()
    {
        var dto = new UpdateProjectDto
        {
            Id = Guid.NewGuid(),
            Title = "Projeto válido",
            Description = "Descrição válida do projeto com mais de dez caracteres.",
            ImageUrl = "https://exemplo.com/a.png",
            Year = DateTime.Now.Year,
            Technologies = new() { "  " }
        };

        var resultado = new UpdateProjectCommandValidator().Validate(new UpdateProjectCommand(dto));

        resultado.Errors.Should().Contain(e => e.PropertyName.Contains("Technologies"));
    }

    private static CreateProjectDto DtoValidoComTecnologias(params string[] tecnologias) => new()
    {
        Title = "Projeto válido",
        Description = "Descrição válida do projeto com mais de dez caracteres.",
        ImageUrl = "https://exemplo.com/a.png",
        Year = DateTime.Now.Year,
        Technologies = tecnologias.ToList()
    };
}
