// ====================================
// Título: GalleryImageMappingProfile.cs
// Descrição: Profile do AutoMapper para a entidade GalleryImage
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.GalleryImages;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade GalleryImage
/// Define como converter GalleryImage (Entity) ↔ DTOs
/// </summary>
public class GalleryImageMappingProfile : Profile
{
    public GalleryImageMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: GalleryImage → GalleryImageDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API
        // Conversão direta - todos os campos têm o mesmo nome
        CreateMap<GalleryImage, GalleryImageDto>();

        // ==========================================
        // MAPEAMENTO: GalleryImage → GalleryImageCardDto
        // ==========================================
        // Usado para listagens (grid de galeria)
        // Conversão direta - AutoMapper mapeia apenas campos presentes no DTO
        CreateMap<GalleryImage, GalleryImageCardDto>();

        // ==========================================
        // MAPEAMENTO: CreateGalleryImageDto → GalleryImage
        // ==========================================
        // Usado quando recebemos dados do POST e criamos uma entidade
        CreateMap<CreateGalleryImageDto, GalleryImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // ==========================================
        // MAPEAMENTO: UpdateGalleryImageDto → GalleryImage
        // ==========================================
        // Usado quando recebemos dados do PUT e atualizamos uma entidade
        CreateMap<UpdateGalleryImageDto, GalleryImage>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Não altera data de criação
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // UpdatedAt será setado no handler
    }
}