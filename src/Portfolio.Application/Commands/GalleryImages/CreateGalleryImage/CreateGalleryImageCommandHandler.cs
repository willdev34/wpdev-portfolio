// ====================================
// Título: CreateGalleryImageCommandHandler.cs
// Descrição: Handler que processa CreateGalleryImageCommand e cria a imagem no banco
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.GalleryImages;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.GalleryImages.CreateGalleryImage;

/// <summary>
/// Handler responsável por processar o command CreateGalleryImageCommand
/// 1. Converte DTO para Entity
/// 2. Salva no banco via Repository
/// 3. Retorna o GalleryImageDto da imagem criada
/// </summary>
public class CreateGalleryImageCommandHandler : IRequestHandler<CreateGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryImageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateGalleryImageCommandHandler(
        IGalleryImageRepository repository,
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
    public async Task<GalleryImageDto> Handle(
        CreateGalleryImageCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. CONVERTER DTO → ENTITY
        // ====================================
        var galleryImage = _mapper.Map<GalleryImage>(request.ImageData);

        // ====================================
        // 2. SALVAR NO BANCO
        // ====================================
        var createdImage = await _repository.AddAsync(galleryImage);
        await _repository.SaveChangesAsync();

        // ====================================
        // 3. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        var galleryImageDto = _mapper.Map<GalleryImageDto>(createdImage);

        return galleryImageDto;
    }
}