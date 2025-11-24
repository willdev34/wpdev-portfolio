// ====================================
// Título: UpdateBlogPostCommandHandler.cs
// Descrição: Handler que processa UpdateBlogPostCommand e atualiza o post
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;

/// <summary>
/// Handler responsável por processar o command UpdateBlogPostCommand
/// 1. Verifica se o post existe
/// 2. Verifica se o slug não está sendo usado por outro post
/// 3. Converte DTO para Entity
/// 4. Atualiza no banco via Repository
/// </summary>
public class UpdateBlogPostCommandHandler : IRequestHandler<UpdateBlogPostCommand, Unit>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public UpdateBlogPostCommandHandler(
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
    public async Task<Unit> Handle(
        UpdateBlogPostCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE O POST EXISTE
        // ====================================
        var existingPost = await _repository.GetByIdAsync(request.PostData.Id);

        if (existingPost == null)
        {
            throw new KeyNotFoundException($"Post com ID {request.PostData.Id} não encontrado");
        }

        // ====================================
        // 2. VERIFICAR SE O SLUG JÁ EXISTE
        // ====================================
        // Verifica se outro post já usa esse slug (excluindo o próprio post)
        var slugExists = await _repository.SlugExistsAsync(
            request.PostData.Slug, 
            excludeId: request.PostData.Id);

        if (slugExists)
        {
            throw new InvalidOperationException(
                $"Já existe outro post com o slug '{request.PostData.Slug}'");
        }

        // ====================================
        // 3. CONVERTER DTO → ENTITY
        // ====================================
        var updatedPost = _mapper.Map(request.PostData, existingPost);

        // ====================================
        // 4. ATUALIZAR NO BANCO
        // ====================================
        await _repository.UpdateAsync(updatedPost);
        await _repository.SaveChangesAsync();

        // ====================================
        // 5. RETORNAR UNIT (VOID)
        // ====================================
        return Unit.Value;
    }
}