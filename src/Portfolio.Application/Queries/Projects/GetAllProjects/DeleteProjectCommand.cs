// ====================================
// Título: DeleteProjectCommand
// Descrição: Command para deletar um projeto (soft delete - CQRS - Write)
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using MediatR;

namespace Portfolio.Application.Commands.Projects.DeleteProject;

/// <summary>
/// Command para deletar um projeto (soft delete - marca como inativo)
/// Recebe apenas o ID do projeto a ser deletado
/// Retorna Unit (void)
/// </summary>
public class DeleteProjectCommand : IRequest<Unit>
{
    // ====================================
    // ID DO PROJETO A SER DELETADO
    // ====================================
    public Guid Id { get; set; }

    // ====================================
    // CONSTRUTOR
    // ====================================
    public DeleteProjectCommand(Guid id)
    {
        Id = id;
    }
}