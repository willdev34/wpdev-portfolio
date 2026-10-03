// Título: BlogPostVisibilityQueryTests.cs
// Descrição: Testes unitários da regra pública nas queries de BlogPosts
//            (slug, id público, id admin e listagem pública)

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostById;
using Portfolio.Application.Queries.BlogPosts.GetBlogPostBySlug;
using Portfolio.Application.Queries.BlogPosts.GetPublicBlogPostById;
using Portfolio.Application.Queries.BlogPosts.GetPublicBlogPosts;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.UnitTests.BlogPosts;

/// <summary>
/// Monta posts em cada situação de visibilidade, com margem folgada de dias
/// (o código usa DateTime.UtcNow direto, sem IClock)
/// </summary>
internal static class VisibilityPosts
{
    public static BlogPost Draft() => Build(BlogPostStatus.Draft, scheduledAt: null);
    public static BlogPost ScheduledFuturo() => Build(BlogPostStatus.Scheduled, DateTime.UtcNow.AddDays(5));
    public static BlogPost ScheduledVencido() => Build(BlogPostStatus.Scheduled, DateTime.UtcNow.AddDays(-5));
    public static BlogPost Published() => Build(BlogPostStatus.Published, scheduledAt: null);

    public static BlogPost ByName(string nome) => nome switch
    {
        "Draft" => Draft(),
        "ScheduledFuturo" => ScheduledFuturo(),
        "ScheduledVencido" => ScheduledVencido(),
        "Published" => Published(),
        _ => throw new ArgumentOutOfRangeException(nameof(nome))
    };

    private static BlogPost Build(BlogPostStatus status, DateTime? scheduledAt) => new()
    {
        Id = Guid.NewGuid(),
        Title = $"Post {status}",
        Slug = $"post-{status.ToString().ToLower()}",
        Status = status,
        ScheduledAt = scheduledAt,
        PublishedAt = scheduledAt ?? DateTime.UtcNow.AddDays(-10)
    };
}

public class GetBlogPostBySlugVisibilityTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetBlogPostBySlugQueryHandler _handler;

    public GetBlogPostBySlugVisibilityTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetBlogPostBySlugQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("ScheduledFuturo")]
    public async Task Handle_DeveRetornarNullSemIncrementarViewCount_QuandoPostNaoPublico(string situacao)
    {
        // Arrange
        var post = VisibilityPosts.ByName(situacao);
        _repositoryMock.Setup(r => r.GetBySlugAsync(post.Slug)).ReturnsAsync(post);

        // Act
        var resultado = await _handler.Handle(new GetBlogPostBySlugQuery(post.Slug), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        _repositoryMock.Verify(r => r.IncrementViewCountAsync(It.IsAny<Guid>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<BlogPostDto>(It.IsAny<BlogPost>()), Times.Never);
    }

    [Theory]
    [InlineData("Published")]
    [InlineData("ScheduledVencido")]
    public async Task Handle_DeveRetornarPostEIncrementarViewCount_QuandoPostPublico(string situacao)
    {
        // Arrange
        var post = VisibilityPosts.ByName(situacao);
        var dto = new BlogPostDto { Id = post.Id, Title = post.Title };
        _repositoryMock.Setup(r => r.GetBySlugAsync(post.Slug)).ReturnsAsync(post);
        _repositoryMock.Setup(r => r.IncrementViewCountAsync(post.Id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(post)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetBlogPostBySlugQuery(post.Slug), CancellationToken.None);

        // Assert
        resultado.Should().BeSameAs(dto);
        _repositoryMock.Verify(r => r.IncrementViewCountAsync(post.Id), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}

public class GetPublicBlogPostByIdQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetPublicBlogPostByIdQueryHandler _handler;

    public GetPublicBlogPostByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetPublicBlogPostByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("ScheduledFuturo")]
    public async Task Handle_DeveRetornarNull_QuandoPostNaoPublico(string situacao)
    {
        // Arrange
        var post = VisibilityPosts.ByName(situacao);
        _repositoryMock.Setup(r => r.GetByIdAsync(post.Id)).ReturnsAsync(post);

        // Act
        var resultado = await _handler.Handle(new GetPublicBlogPostByIdQuery(post.Id), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        _mapperMock.Verify(m => m.Map<BlogPostDto>(It.IsAny<BlogPost>()), Times.Never);
    }

    [Theory]
    [InlineData("Published")]
    [InlineData("ScheduledVencido")]
    public async Task Handle_DeveRetornarPost_QuandoPostPublico(string situacao)
    {
        // Arrange
        var post = VisibilityPosts.ByName(situacao);
        var dto = new BlogPostDto { Id = post.Id, Title = post.Title };
        _repositoryMock.Setup(r => r.GetByIdAsync(post.Id)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(post)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetPublicBlogPostByIdQuery(post.Id), CancellationToken.None);

        // Assert
        resultado.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task Handle_DeveRetornarNull_QuandoPostNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BlogPost?)null);

        // Act
        var resultado = await _handler.Handle(new GetPublicBlogPostByIdQuery(id), CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }
}

public class GetBlogPostByIdAdminVisibilityTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetBlogPostByIdQueryHandler _handler;

    public GetBlogPostByIdAdminVisibilityTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetBlogPostByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("ScheduledFuturo")]
    [InlineData("ScheduledVencido")]
    [InlineData("Published")]
    public async Task Handle_DeveRetornarPostEmQualquerStatus_QuandoQueryDoAdmin(string situacao)
    {
        // Arrange
        // GetBlogPostByIdQuery é a query do admin (GET api/blogposts/admin/{id}), sem filtro de visibilidade
        var post = VisibilityPosts.ByName(situacao);
        var dto = new BlogPostDto { Id = post.Id, Status = post.Status.ToString() };
        _repositoryMock.Setup(r => r.GetByIdAsync(post.Id)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(post)).Returns(dto);

        // Act
        var resultado = await _handler.Handle(new GetBlogPostByIdQuery(post.Id), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be(post.Status.ToString());
    }
}

public class GetPublicBlogPostsQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetPublicBlogPostsQueryHandler _handler;

    public GetPublicBlogPostsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetPublicBlogPostsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveUsarGetPublishedAsync_ENuncaGetAllAsync()
    {
        // Arrange
        var posts = new List<BlogPost> { VisibilityPosts.Published(), VisibilityPosts.ScheduledVencido() };
        var cards = posts.Select(p => new BlogPostCardDto { Id = p.Id, Title = p.Title }).ToList();
        _repositoryMock.Setup(r => r.GetPublishedAsync()).ReturnsAsync(posts);
        _mapperMock.Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(posts)).Returns(cards);

        // Act
        var resultado = await _handler.Handle(new GetPublicBlogPostsQuery(), CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(cards);
        _repositoryMock.Verify(r => r.GetPublishedAsync(), Times.Once);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }
}
