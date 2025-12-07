// ====================================
// Título: DeleteGalleryImageCommandHandler.cs
// Descrição: Handler que processa DeleteGalleryImageCommand e marca imagem como invisível
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.GalleryImages.DeleteGalleryImage;

public class DeleteGalleryImageCommandHandler : IRequestHandler<DeleteGalleryImageCommand, Unit>
{
    private readonly IGalleryImageRepository _repository;

    public DeleteGalleryImageCommandHandler(IGalleryImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteGalleryImageCommand request, 
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(request.Id);

        if (!exists)
        {
            throw new KeyNotFoundException($"Imagem com ID {request.Id} não encontrada");
        }

        await _repository.DeleteAsync(request.Id);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}