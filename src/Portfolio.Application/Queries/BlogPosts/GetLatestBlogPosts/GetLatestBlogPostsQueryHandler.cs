// ====================================
// Título: GetLatestBlogPostsQueryHandler.cs
// Descrição: Handler que retorna os últimos posts PÚBLICOS
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.BlogPosts.GetLatestBlogPosts;

/// <summary>
/// Handler responsável por processar a query GetLatestBlogPostsQuery
/// Busca os N posts publicos mais recentes e converte para BlogPostCardDto
/// A regra de visibilidade e a ordenacao ficam no Repository
/// </summary>
public class GetLatestBlogPostsQueryHandler : IRequestHandler<GetLatestBlogPostsQuery, IEnumerable<BlogPostCardDto>>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    public GetLatestBlogPostsQueryHandler(IBlogPostRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogPostCardDto>> Handle(
        GetLatestBlogPostsQuery request,
        CancellationToken cancellationToken)
    {
        var blogPosts = await _repository.GetLatestPublishedAsync(request.Count);

        return _mapper.Map<IEnumerable<BlogPostCardDto>>(blogPosts);
    }
}
