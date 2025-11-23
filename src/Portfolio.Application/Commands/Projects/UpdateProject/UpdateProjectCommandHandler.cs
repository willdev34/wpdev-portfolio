// ====================================
// Título: UpdateProjectCommandHandler
// Descrição: Handler que processa UpdateProjectCommand e atualiza o projeto
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.Projects.UpdateProject;

/// <summary>
/// Handler responsável por processar o command UpdateProjectCommand
/// 1. Verifica se o projeto existe
/// 2. Converte DTO para Entity
/// 3. Atualiza no banco via Repository
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public UpdateProjectCommandHandler(
        IProjectRepository repository,
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
        UpdateProjectCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE O PROJETO EXISTE
        // ====================================
        var existingProject = await _repository.GetByIdAsync(request.ProjectData.Id);

        if (existingProject == null)
        {
            throw new KeyNotFoundException($"Projeto com ID {request.ProjectData.Id} não encontrado");
        }

        // ====================================
        // 2. CONVERTER DTO → ENTITY
        // ====================================
        var updatedProject = _mapper.Map(request.ProjectData, existingProject);

        // ====================================
        // 3. ATUALIZAR NO BANCO
        // ====================================
        await _repository.UpdateAsync(updatedProject);
        await _repository.SaveChangesAsync();

        // ====================================
        // 4. RETORNAR UNIT (VOID)
        // ====================================
        return Unit.Value;
    }
}