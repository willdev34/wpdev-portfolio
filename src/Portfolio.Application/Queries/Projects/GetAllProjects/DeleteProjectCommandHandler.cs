// ====================================
// Título: DeleteProjectCommandHandler
// Descrição: Handler que processa DeleteProjectCommand e marca projeto como inativo
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.Projects.DeleteProject;

/// <summary>
/// Handler responsável por processar o command DeleteProjectCommand
/// Implementa SOFT DELETE - marca o projeto como inativo ao invés de deletar fisicamente
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
    private readonly IProjectRepository _repository;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public DeleteProjectCommandHandler(IProjectRepository repository)
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
        DeleteProjectCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE O PROJETO EXISTE
        // ====================================
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Projeto com ID {request.Id} não encontrado");
        }

        // ====================================
        // 2. SOFT DELETE (marca como inativo)
        // ====================================
        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        // ====================================
        // 3. RETORNAR UNIT (VOID)
        // ====================================
        return Unit.Value;
    }
}