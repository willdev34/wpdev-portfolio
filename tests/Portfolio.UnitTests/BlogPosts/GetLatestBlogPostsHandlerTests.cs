// Título: GetLatestBlogPostsHandlerTests.cs
// Descrição: Testes unitários do handler da busca dos últimos posts públicos.
//            A regra de visibilidade e a ordenação ficam no Repository (cobertas na integração);
//            aqui valida repasse do count, mapeamento e preservação da ordem

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.BlogPosts.GetLatestBlogPosts;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.BlogPosts;

public class GetLatestBlogPostsQueryHandlerTests
{
    private readonly Mock<IBlogPostRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetLatestBlogPostsQueryHandler _handler;

    public GetLatestBlogPostsQueryHandlerTests()
    {
        _handler = new GetLatestBlogPostsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(12)]
    public async Task Handle_DeveRepassarOCountAoRepositorio(int count)
    {
        _repositoryMock.Setup(r => r.GetLatestPublishedAsync(count))
            .ReturnsAsync(new List<BlogPost>());
        _mapperMock.Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(It.IsAny<IEnumerable<BlogPost>>()))
            .Returns(new List<BlogPostCardDto>());

        await _handler.Handle(new GetLatestBlogPostsQuery(count), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetLatestPublishedAsync(count), Times.Once);
        _repositoryMock.Verify(r => r.GetPublishedAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveMapearParaBlogPostCardDtoMantendoAOrdemDoRepositorio()
    {
        // Ordem devolvida pelo repositório: a mais recente primeiro
        var posts = new List<BlogPost>
        {
            new() { Id = Guid.NewGuid(), Title = "Mais recente", Slug = "mais-recente" },
            new() { Id = Guid.NewGuid(), Title = "Do meio", Slug = "do-meio" },
            new() { Id = Guid.NewGuid(), Title = "Mais antigo", Slug = "mais-antigo" }
        };
        var dtos = posts.Select(p => new BlogPostCardDto { Id = p.Id, Title = p.Title, Slug = p.Slug }).ToList();

        _repositoryMock.Setup(r => r.GetLatestPublishedAsync(3)).ReturnsAsync(posts);
        _mapperMock.Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(posts)).Returns(dtos);

        var result = (await _handler.Handle(new GetLatestBlogPostsQuery(3), CancellationToken.None)).ToList();

        // O handler entrega ao mapper a mesma sequência do repositório, sem reordenar
        _mapperMock.Verify(m => m.Map<IEnumerable<BlogPostCardDto>>(posts), Times.Once);
        result.Select(r => r.Slug).Should().Equal("mais-recente", "do-meio", "mais-antigo");
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoHaPostsPublicos()
    {
        _repositoryMock.Setup(r => r.GetLatestPublishedAsync(3)).ReturnsAsync(new List<BlogPost>());
        _mapperMock.Setup(m => m.Map<IEnumerable<BlogPostCardDto>>(It.IsAny<IEnumerable<BlogPost>>()))
            .Returns(new List<BlogPostCardDto>());

        var result = await _handler.Handle(new GetLatestBlogPostsQuery(3), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
