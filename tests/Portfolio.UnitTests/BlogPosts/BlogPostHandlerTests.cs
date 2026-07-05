// Título: BlogPostHandlerTests.cs
// Descrição: Testes unitários dos handlers de BlogPosts (Query e Command)

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.BlogPosts.CreateBlogPost;
using Portfolio.Application.Commands.BlogPosts.DeleteBlogPost;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.BlogPosts.GetAllBlogPosts;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostById;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.BlogPosts;

public class GetAllBlogPostsQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllBlogPostsQueryHandler _handler;

    public GetAllBlogPostsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllBlogPostsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDePosts_QuandoExistiremPosts()
    {
        // Arrange
        var posts = new List<BlogPost>
        {
            new() { Id = Guid.NewGuid(), Title = "Clean Architecture em .NET" },
            new() { Id = Guid.NewGuid(), Title = "CQRS na prática" }
        };

        var dtos = new List<BlogPostCardDto>
        {
            new() { Id = posts[0].Id, Title = "Clean Architecture em .NET" },
            new() { Id = posts[1].Id, Title = "CQRS na prática" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(posts);
        _mapperMock.Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(posts)).Returns(dtos);

        // Act
        var resultado = await _handler.Handle(new GetAllBlogPostsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Title.Should().Be("Clean Architecture em .NET");
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremPosts()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BlogPost>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(It.IsAny<IEnumerable<BlogPost>>()))
            .Returns(new List<BlogPostCardDto>());

        // Act
        var resultado = await _handler.Handle(new GetAllBlogPostsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveChamarGetAllAsync_UmaVez()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BlogPost>());
        _mapperMock
            .Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(It.IsAny<IEnumerable<BlogPost>>()))
            .Returns(new List<BlogPostCardDto>());

        // Act
        await _handler.Handle(new GetAllBlogPostsQuery(), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
}

public class GetBlogPostByIdQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetBlogPostByIdQueryHandler _handler;

    public GetBlogPostByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetBlogPostByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPost_QuandoPostExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        var post = new BlogPost { Id = id, Title = "Clean Architecture em .NET" };
        var dto = new BlogPostDto { Id = id, Title = "Clean Architecture em .NET" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(post)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetBlogPostByIdQuery(id), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
        resultado.Title.Should().Be("Clean Architecture em .NET");
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoPostNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BlogPost?)null);
        _mapperMock.Setup(m => m.Map<BlogPostDto?>(null)).Returns((BlogPostDto?)null);

        // Act
        var resultado = await _handler.Handle(new GetBlogPostByIdQuery(id), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }
}

public class DeleteBlogPostCommandHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly DeleteBlogPostCommandHandler _handler;

    public DeleteBlogPostCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _handler = new DeleteBlogPostCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveDeletarPost_QuandoPostExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(new DeleteBlogPostCommand(id), CancellationToken.None);

        // Assert
        resultado.Should().Be(MediatR.Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoPostNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(new DeleteBlogPostCommand(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}