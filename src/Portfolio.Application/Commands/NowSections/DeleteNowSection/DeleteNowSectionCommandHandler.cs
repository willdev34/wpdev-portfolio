// ====================================
// Título: DeleteNowSectionCommandHandler.cs
// Descrição: Handler que processa DeleteNowSectionCommand e marca seção como inativa
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.NowSections.DeleteNowSection;

public class DeleteNowSectionCommandHandler : IRequestHandler<DeleteNowSectionCommand, Unit>
{
    private readonly INowSectionRepository _repository;

    public DeleteNowSectionCommandHandler(INowSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteNowSectionCommand request, 
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Seção com ID {request.Id} não encontrada");
        }

        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}