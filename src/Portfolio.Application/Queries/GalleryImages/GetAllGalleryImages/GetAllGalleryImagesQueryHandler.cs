// ====================================
// Título: GetAllGalleryImagesQueryHandler.cs
// Descrição: Handler que processa GetAllGalleryImagesQuery e retorna as imagens
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.GalleryImages;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.GalleryImages.GetAllGalleryImages;

/// <summary>
/// Handler responsável por processar a query GetAllGalleryImagesQuery
/// 1. Busca as imagens no banco (via Repository)
/// 2. Converte de Entity para DTO (via AutoMapper)
/// 3. Retorna a lista de GalleryImageCardDto
/// </summary>
public class GetAllGalleryImagesQueryHandler : IRequestHandler<GetAllGalleryImagesQuery, IEnumerable<GalleryImageCardDto>>
{
    private readonly IGalleryImageRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GetAllGalleryImagesQueryHandler(
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
    public async Task<IEnumerable<GalleryImageCardDto>> Handle(
        GetAllGalleryImagesQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Busca TODAS as imagens do banco (ordenadas por Order)
        var galleryImages = await _repository.GetAllAsync();

        // 2. Converte de GalleryImage (Entity) para GalleryImageCardDto
        var galleryImageCards = _mapper.Map<IEnumerable<GalleryImageCardDto>>(galleryImages);

        // 3. Retorna a lista de DTOs
        return galleryImageCards;
    }
}