// ====================================
// Título: GetGalleryImageByIdQueryHandler.cs
// Descrição: Handler que processa GetGalleryImageByIdQuery e retorna a imagem
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.GalleryImages;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.GalleryImages.GetGalleryImageById;

/// <summary>
/// Handler responsável por processar a query GetGalleryImageByIdQuery
/// 1. Busca a imagem no banco por ID (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna o GalleryImageDto ou null se não encontrado
/// </summary>
public class GetGalleryImageByIdQueryHandler : IRequestHandler<GetGalleryImageByIdQuery, GalleryImageDto?>
{
    private readonly IGalleryImageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetGalleryImageByIdQueryHandler(
        IGalleryImageRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa a Query
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica da query
    /// </summary>
    public async Task<GalleryImageDto?> Handle(
        GetGalleryImageByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca a imagem no banco por ID
        var galleryImage = await _repository.GetByIdAsync(request.Id);

        // 2. Se não encontrou, retorna null
        if (galleryImage == null)
        {
            return null;
        }

        // 3. Converte de GalleryImage (Entity) para GalleryImageDto
        var galleryImageDto = _mapper.Map<GalleryImageDto>(galleryImage);

        // 4. Retorna o DTO
        return galleryImageDto;
    }
}