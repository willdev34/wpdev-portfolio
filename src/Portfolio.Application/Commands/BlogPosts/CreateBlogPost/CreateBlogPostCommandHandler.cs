// ====================================
// Título: CreateBlogPostCommandHandler.cs
// Descrição: Handler que processa CreateBlogPostCommand e cria o post no banco
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Commands.BlogPosts.CreateBlogPost;

/// <summary>
/// Handler responsável por processar o command CreateBlogPostCommand
/// 1. Verifica se o Slug já existe (evita URLs duplicadas)
/// 2. Converte DTO para Entity
/// 3. Salva no banco via Repository
/// 4. Retorna o BlogPostDto do post criado
/// </summary>
public class CreateBlogPostCommandHandler : IRequestHandler<CreateBlogPostCommand, BlogPostDto>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateBlogPostCommandHandler(
        IBlogPostRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa o Command
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica do command
    /// </summary>
    public async Task<BlogPostDto> Handle(
        CreateBlogPostCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE O SLUG JÁ EXISTE
        // ====================================
        // Evita criar posts com URLs duplicadas
        var slugExists = await _repository.SlugExistsAsync(request.PostData.Slug);

        if (slugExists)
        {
            throw new InvalidOperationException($"Já existe um post com o slug '{request.PostData.Slug}'");
        }

        // ====================================
        // 2. CONVERTER DTO → ENTITY
        // ====================================
        var blogPost = _mapper.Map<BlogPost>(request.PostData);

        // ====================================
        // 2.1 SINCRONIZAR Status COM IsPublished / PublishedAt / ScheduledAt
        // ====================================
        // Compatibilidade: o formulário atual do admin ainda não envia Status,
        // então derivamos de IsPublished quando Status vier nulo
        var status = request.PostData.Status
            ?? (request.PostData.IsPublished ? BlogPostStatus.Published : BlogPostStatus.Draft);

        blogPost.Status = status;

        switch (status)
        {
            case BlogPostStatus.Published:
                blogPost.IsPublished = true;
                blogPost.PublishedAt ??= DateTime.UtcNow;
                blogPost.ScheduledAt = null;
                break;
            case BlogPostStatus.Scheduled:
                blogPost.IsPublished = false;
                blogPost.ScheduledAt = request.PostData.ScheduledAt;
                blogPost.PublishedAt = request.PostData.ScheduledAt;
                break;
            case BlogPostStatus.Draft:
            default:
                blogPost.IsPublished = false;
                blogPost.ScheduledAt = null;
                break;
        }

        // ====================================
        // 3. SALVAR NO BANCO
        // ====================================
        // O Repository cuida de gerar o ID e setar CreatedAt
        var createdPost = await _repository.AddAsync(blogPost);
        
        // Salva as mudanças no banco
        await _repository.SaveChangesAsync();

        // ====================================
        // 4. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        var blogPostDto = _mapper.Map<BlogPostDto>(createdPost);

        return blogPostDto;
    }
}