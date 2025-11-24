// ====================================
// Título: DeleteBlogPostCommandHandler.cs
// Descrição: Handler que processa DeleteBlogPostCommand e deleta o post FISICAMENTE
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.BlogPosts.DeleteBlogPost;

/// <summary>
/// Handler responsável por processar o command DeleteBlogPostCommand
/// ATENÇÃO: Implementa HARD DELETE - deleta fisicamente do banco
/// (ao contrário de Project que usa soft delete)
/// </summary>
public class DeleteBlogPostCommandHandler : IRequestHandler<DeleteBlogPostCommand, Unit>
{
    private readonly IBlogPostRepository _repository;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public DeleteBlogPostCommandHandler(IBlogPostRepository repository)
    {
        _repository = repository;
    }

    // ====================================
    // HANDLE - Processa o Command
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica do command
    /// </summary>
    public async Task<Unit> Handle(
        DeleteBlogPostCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE O POST EXISTE
        // ====================================
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Post com ID {request.Id} não encontrado");
        }

        // ====================================
        // 2. HARD DELETE (deleta fisicamente)
        // ====================================
        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        // ====================================
        // 3. RETORNAR UNIT (VOID)
        // ====================================
        return Unit.Value;
    }
}