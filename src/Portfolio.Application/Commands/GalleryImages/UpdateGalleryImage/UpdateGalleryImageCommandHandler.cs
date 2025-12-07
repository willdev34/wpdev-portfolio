// ====================================
// Título: UpdateGalleryImageCommandHandler.cs
// Descrição: Handler que processa UpdateGalleryImageCommand e atualiza a imagem
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.GalleryImages.UpdateGalleryImage;

public class UpdateGalleryImageCommandHandler : IRequestHandler<UpdateGalleryImageCommand, Unit>
{
    private readonly IGalleryImageRepository _repository;
    private readonly IMapper _mapper;

    public UpdateGalleryImageCommandHandler(
        IGalleryImageRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        UpdateGalleryImageCommand request, 
        CancellationToken cancellationToken)
    {
        var existingImage = await _repository.GetByIdAsync(request.ImageData.Id);

        if (existingImage == null)
        {
            throw new KeyNotFoundException($"Imagem com ID {request.ImageData.Id} não encontrada");
        }

        var updatedImage = _mapper.Map(request.ImageData, existingImage);

        await _repository.UpdateAsync(updatedImage);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}