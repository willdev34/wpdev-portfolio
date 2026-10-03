// Título: BulkDeleteBlogPostsHandlerTests.cs
// Descrição: Testes unitários do handler da exclusão em massa de posts

using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.BlogPosts.BulkDeleteBlogPosts;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.UnitTests.BlogPosts;

public class BulkDeleteBlogPostsCommandHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock = new();
    private readonly BulkDeleteBlogPostsCommandHandler _handler;

    public BulkDeleteBlogPostsCommandHandlerTests()
    {
        _handler = new BulkDeleteBlogPostsCommandHandler(_repositoryMock.Object);
    }

    private static BulkDeleteBlogPostsCommand BuildCommand(params Guid[] ids) =>
        new(new BulkDeleteBlogPostsDto { Ids = ids.ToList() });

    [Fact]
    public async Task Handle_DeveExcluirTodos_QuandoTodosExistem()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        var deleted = await _handler.Handle(BuildCommand(ids), CancellationToken.None);

        deleted.Should().Be(3);
        foreach (var id in ids)
            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveIgnorarDuplicados()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);

        var deleted = await _handler.Handle(BuildCommand(id, id, id), CancellationToken.None);

        deleted.Should().Be(1);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoDeveContarNemExcluirInexistentes()
    {
        var existente = Guid.NewGuid();
        var inexistente = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(existente)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsAsync(inexistente)).ReturnsAsync(false);

        var deleted = await _handler.Handle(BuildCommand(existente, inexistente), CancellationToken.None);

        deleted.Should().Be(1);
        _repositoryMock.Verify(r => r.DeleteAsync(existente), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(inexistente), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveChamarSaveChangesUmaUnicaVez()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        await _handler.Handle(BuildCommand(ids), CancellationToken.None);

        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoDeveChamarSaveChanges_QuandoNadaFoiExcluido()
    {
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var deleted = await _handler.Handle(BuildCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        deleted.Should().Be(0);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
