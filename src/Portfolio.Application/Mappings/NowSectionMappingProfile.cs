// ====================================
// Título: NowSectionMappingProfile.cs
// Descrição: Profile do AutoMapper para a entidade NowSection
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade NowSection
/// Define como converter NowSection (Entity) ↔ DTOs
/// </summary>
public class NowSectionMappingProfile : Profile
{
    public NowSectionMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: NowSection → NowSectionDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API
        // Conversão direta - todos os campos têm o mesmo nome
        CreateMap<NowSection, NowSectionDto>();

        // ==========================================
        // MAPEAMENTO: CreateNowSectionDto → NowSection
        // ==========================================
        // Usado quando recebemos dados do POST e criamos uma entidade
        CreateMap<CreateNowSectionDto, NowSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Novo sempre ativo
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no repository
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // UpdatedAt será setado no repository

        // ==========================================
        // MAPEAMENTO: UpdateNowSectionDto → NowSection
        // ==========================================
        // Usado quando recebemos dados do PUT e atualizamos uma entidade
        CreateMap<UpdateNowSectionDto, NowSection>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Não altera data de criação
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // UpdatedAt será setado no repository
    }
}