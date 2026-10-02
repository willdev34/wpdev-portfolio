// ====================================
// Título: UpdateBlogPostCommandHandler.cs
// Descrição: Handler que processa UpdateBlogPostCommand e atualiza o post
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Enums;

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
        // 3.1 SINCRONIZAR Status COM IsPublished / PublishedAt / ScheduledAt
        // ====================================
        // Compatibilidade: o formulário atual do admin ainda não envia Status,
        // então derivamos de IsPublished quando Status vier nulo
        var status = request.PostData.Status
            ?? (request.PostData.IsPublished ? BlogPostStatus.Published : BlogPostStatus.Draft);

        updatedPost.Status = status;

        switch (status)
        {
            case BlogPostStatus.Published:
                updatedPost.IsPublished = true;
                // Se nunca foi publicado ou a data e futura (ex: veio de Scheduled), usa agora.
                // Se ja e uma data passada, preserva (historico de quem voltou de rascunho).
                if (updatedPost.PublishedAt == null || updatedPost.PublishedAt > DateTime.UtcNow)
                {
                    updatedPost.PublishedAt = DateTime.UtcNow;
                }
                updatedPost.ScheduledAt = null;
                break;
            case BlogPostStatus.Scheduled:
                updatedPost.IsPublished = false;
                updatedPost.ScheduledAt = request.PostData.ScheduledAt;
                updatedPost.PublishedAt = request.PostData.ScheduledAt;
                break;
            case BlogPostStatus.Draft:
            default:
                updatedPost.IsPublished = false;
                updatedPost.ScheduledAt = null;
                break;
        }

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