// Título: RemainingHandlerTests.cs
// Descrição: Testes unitários dos handlers restantes - GetById, Update, Create, Delete

using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Portfolio.Application.Commands.BlogPosts.CreateBlogPost;
using Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;
using Portfolio.Application.Commands.ContactMessages.CreateContactMessage;
using Portfolio.Application.Commands.NowSections.CreateNowSection;
using Portfolio.Application.Commands.NowSections.DeleteNowSection;
using Portfolio.Application.Commands.NowSections.UpdateNowSection;
using Portfolio.Application.Commands.Projects.DeleteProject;
using Portfolio.Application.Commands.Projects.UpdateProject;
using Portfolio.Application.Commands.TimelineEvents.UpdateTimelineEvent;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostBySlug;
using Portfolio.Application.Queries.ContactMessages.GetContactMessageById;
using Portfolio.Application.Queries.NowSections.GetNowSectionById;
using Portfolio.Application.Queries.Projects.GetProjectById;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.Remaining;

// ==========================================
// PROJECTS
// ==========================================

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProjectByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarProjeto_QuandoProjetoExistir()
    {
        var id = Guid.NewGuid();
        var projeto = new Project { Id = id, Title = "DrenaMais" };
        var dto = new ProjectDto { Id = id, Title = "DrenaMais" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(projeto);
        _mapperMock.Setup(m => m.Map<ProjectDto>(projeto)).Returns(dto);

        var resultado = await _handler.Handle(new GetProjectByIdQuery(id), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoProjetoNaoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Project?)null);
        _mapperMock.Setup(m => m.Map<ProjectDto?>(null)).Returns((ProjectDto?)null);

        var resultado = await _handler.Handle(new GetProjectByIdQuery(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _handler = new DeleteProjectCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveDeletarProjeto_QuandoProjetoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(new DeleteProjectCommand(id), CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoProjetoNaoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        var act = async () => await _handler.Handle(new DeleteProjectCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateProjectCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAtualizarProjeto_QuandoProjetoExistir()
    {
        var id = Guid.NewGuid();
        var projeto = new Project { Id = id, Title = "DrenaMais" };
        var updateDto = new UpdateProjectDto { Id = id, Title = "DrenaMais v2" };
        var command = new UpdateProjectCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(projeto);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoProjetoNaoExistir()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateProjectDto { Id = id, Title = "Teste" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Project?)null);

        var act = async () => await _handler.Handle(new UpdateProjectCommand(updateDto), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

// ==========================================
// BLOGPOSTS
// ==========================================

public class GetBlogPostBySlugQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetBlogPostBySlugQueryHandler _handler;

    public GetBlogPostBySlugQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetBlogPostBySlugQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPost_QuandoSlugExistir()
    {
        var slug = "clean-architecture-dotnet";
        var post = new BlogPost { Id = Guid.NewGuid(), Title = "Clean Architecture", Slug = slug };
        var dto = new BlogPostDto { Id = post.Id, Title = "Clean Architecture" };

        _repositoryMock.Setup(r => r.GetBySlugAsync(slug)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(post)).Returns(dto);

        var resultado = await _handler.Handle(new GetBlogPostBySlugQuery(slug), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Title.Should().Be("Clean Architecture");
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoSlugNaoExistir()
    {
        var slug = "slug-inexistente";
        _repositoryMock.Setup(r => r.GetBySlugAsync(slug)).ReturnsAsync((BlogPost?)null);
        _mapperMock.Setup(m => m.Map<BlogPostDto?>(null)).Returns((BlogPostDto?)null);

        var resultado = await _handler.Handle(new GetBlogPostBySlugQuery(slug), CancellationToken.None);

        resultado.Should().BeNull();
    }
}

public class CreateBlogPostCommandHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateBlogPostCommandHandler _handler;

    public CreateBlogPostCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateBlogPostCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarPost_ERetornarDto()
    {
        var createDto = new CreateBlogPostDto { Title = "Novo Post", Excerpt = "Resumo" };
        var command = new CreateBlogPostCommand(createDto);
        var entity = new BlogPost { Id = Guid.NewGuid(), Title = "Novo Post" };
        var dto = new BlogPostDto { Id = entity.Id, Title = "Novo Post" };

        _mapperMock.Setup(m => m.Map<BlogPost>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(entity)).Returns(dto);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.Title.Should().Be("Novo Post");
        _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoSlugJaExistir()
    {
        // Arrange
        var createDto = new CreateBlogPostDto { Title = "Novo Post", Slug = "slug-existente" };
        var command = new CreateBlogPostCommand(createDto);

        _repositoryMock
            .Setup(r => r.SlugExistsAsync("slug-existente", null))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<BlogPost>()), Times.Never);
    }
}

public class UpdateBlogPostCommandHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateBlogPostCommandHandler _handler;

    public UpdateBlogPostCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateBlogPostCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAtualizarPost_QuandoPostExistir()
    {
        var id = Guid.NewGuid();
        var post = new BlogPost { Id = id, Title = "Post Antigo" };
        var updateDto = new UpdateBlogPostDto { Id = id, Title = "Post Atualizado" };
        var command = new UpdateBlogPostCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BlogPost>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoPostNaoExistir()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateBlogPostDto { Id = id, Title = "Teste" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BlogPost?)null);

        var act = async () => await _handler.Handle(new UpdateBlogPostCommand(updateDto), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

// ==========================================
// CONTACT MESSAGES
// ==========================================

public class GetContactMessageByIdQueryHandlerTests
{
    private readonly Mock<IContactMessageRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetContactMessageByIdQueryHandler _handler;

    public GetContactMessageByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetContactMessageByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarMensagem_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var mensagem = new ContactMessage { Id = id, Name = "Maria", Email = "maria@teste.com" };
        var dto = new ContactMessageDto { Id = id, Name = "Maria" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mensagem);
        _mapperMock.Setup(m => m.Map<ContactMessageDto>(mensagem)).Returns(dto);

        var resultado = await _handler.Handle(new GetContactMessageByIdQuery(id), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ContactMessage?)null);
        _mapperMock.Setup(m => m.Map<ContactMessageDto?>(null)).Returns((ContactMessageDto?)null);

        var resultado = await _handler.Handle(new GetContactMessageByIdQuery(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}

public class CreateContactMessageCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly CreateContactMessageCommandHandler _handler;

    public CreateContactMessageCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new CreateContactMessageCommandHandler(_repositoryMock.Object, _mapperMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarMensagem_ERetornarDto()
    {
        var createDto = new CreateContactMessageDto
        {
            Name = "João",
            Email = "joao@teste.com",
            Subject = "Parceria",
            Message = "Olá!"
        };
        var command = new CreateContactMessageCommand(createDto);
        var entity = new ContactMessage { Id = Guid.NewGuid(), Name = "João" };
        var dto = new ContactMessageDto { Id = entity.Id, Name = "João" };

        _mapperMock.Setup(m => m.Map<ContactMessage>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<ContactMessageDto>(entity)).Returns(dto);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.Name.Should().Be("João");
        _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

// ==========================================
// NOW SECTIONS
// ==========================================

public class GetNowSectionByIdQueryHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetNowSectionByIdQueryHandler _handler;

    public GetNowSectionByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetNowSectionByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSecao_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var secao = new NowSection { Id = id, IsActive = true };
        var dto = new NowSectionDto { Id = id };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(secao);
        _mapperMock.Setup(m => m.Map<NowSectionDto>(secao)).Returns(dto);

        var resultado = await _handler.Handle(new GetNowSectionByIdQuery(id), CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((NowSection?)null);
        _mapperMock.Setup(m => m.Map<NowSectionDto?>(null)).Returns((NowSectionDto?)null);

        var resultado = await _handler.Handle(new GetNowSectionByIdQuery(id), CancellationToken.None);

        resultado.Should().BeNull();
    }
}

public class CreateNowSectionCommandHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateNowSectionCommandHandler _handler;

    public CreateNowSectionCommandHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateNowSectionCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarSecao_ERetornarDto()
    {
        var createDto = new CreateNowSectionDto();
        var command = new CreateNowSectionCommand(createDto);
        var entity = new NowSection { Id = Guid.NewGuid(), IsActive = true };
        var dto = new NowSectionDto { Id = entity.Id };

        _mapperMock.Setup(m => m.Map<NowSection>(createDto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<NowSectionDto>(entity)).Returns(dto);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

public class DeleteNowSectionCommandHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly DeleteNowSectionCommandHandler _handler;

    public DeleteNowSectionCommandHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _handler = new DeleteNowSectionCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveDeletarSecao_QuandoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(new DeleteNowSectionCommand(id), CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        var act = async () => await _handler.Handle(new DeleteNowSectionCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

public class UpdateNowSectionCommandHandlerTests
{
    private readonly Mock<INowSectionRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateNowSectionCommandHandler _handler;

    public UpdateNowSectionCommandHandlerTests()
    {
        _repositoryMock = new Mock<INowSectionRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateNowSectionCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAtualizarSecao_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var secao = new NowSection { Id = id };
        var updateDto = new UpdateNowSectionDto { Id = id };
        var command = new UpdateNowSectionCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(secao);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<NowSection>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<NowSection>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateNowSectionDto { Id = id };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((NowSection?)null);

        var act = async () => await _handler.Handle(new UpdateNowSectionCommand(updateDto), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeveDesativarTodasSecoes_QuandoAtivandoSecaoInativa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var secao = new NowSection { Id = id, IsActive = false };
        var updateDto = new UpdateNowSectionDto { Id = id, IsActive = true };
        var command = new UpdateNowSectionCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(secao);
        _repositoryMock.Setup(r => r.DeactivateAllAsync()).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<NowSection>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeactivateAllAsync(), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<NowSection>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoDeveDesativarTodasSecoes_QuandoSecaoJaEstaAtiva()
    {
        // Arrange
        var id = Guid.NewGuid();
        var secao = new NowSection { Id = id, IsActive = true };
        var updateDto = new UpdateNowSectionDto { Id = id, IsActive = true };
        var command = new UpdateNowSectionCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(secao);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<NowSection>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeactivateAllAsync(), Times.Never);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<NowSection>()), Times.Once);
    }
}

// ==========================================
// TIMELINE EVENTS - UPDATE
// ==========================================

public class UpdateTimelineEventCommandHandlerTests
{
    private readonly Mock<ITimelineEventRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTimelineEventCommandHandler _handler;

    public UpdateTimelineEventCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITimelineEventRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateTimelineEventCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAtualizarEvento_QuandoExistir()
    {
        var id = Guid.NewGuid();
        var evento = new TimelineEvent { Id = id, Title = "Evento Antigo" };
        var updateDto = new UpdateTimelineEventDto { Id = id, Title = "Evento Atualizado" };
        var command = new UpdateTimelineEventCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(evento);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TimelineEvent>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var resultado = await _handler.Handle(command, CancellationToken.None);

        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TimelineEvent>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoNaoExistir()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateTimelineEventDto { Id = id, Title = "Teste" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TimelineEvent?)null);

        var act = async () => await _handler.Handle(new UpdateTimelineEventCommand(updateDto), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
