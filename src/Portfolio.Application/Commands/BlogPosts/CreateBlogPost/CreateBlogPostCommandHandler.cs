// ====================================
// Título: CreateBlogPostCommandHandler.cs
// Descrição: Handler que processa CreateBlogPostCommand e cria o post no banco
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

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
        // 3. SALVAR NO BANCO
        // ====================================
        // O Repository cuida de:
        // - Gerar o ID
        // - Setar CreatedAt
        // - Setar PublishedAt se IsPublished = true
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