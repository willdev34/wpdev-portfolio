// ====================================
// Título: TimelineEventMappingProfile.cs
// Descrição: Profile do AutoMapper para a entidade TimelineEvent
// Autor: Will
// Empresa: WpDev
// Data: 29/11/2024
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.TimelineEvents;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade TimelineEvent
/// Define como converter TimelineEvent (Entity) ↔ DTOs
/// </summary>
public class TimelineEventMappingProfile : Profile
{
    public TimelineEventMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: TimelineEvent → TimelineEventDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API
        // Converte o enum Type para string
        CreateMap<TimelineEvent, TimelineEventDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // ==========================================
        // MAPEAMENTO: TimelineEvent → TimelineEventCardDto
        // ==========================================
        // Usado para listagens (timeline)
        CreateMap<TimelineEvent, TimelineEventCardDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // ==========================================
        // MAPEAMENTO: CreateTimelineEventDto → TimelineEvent
        // ==========================================
        // Usado quando recebemos dados do POST e criamos uma entidade
        CreateMap<CreateTimelineEventDto, TimelineEvent>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TimelineEventType)src.Type));

        // ==========================================
        // MAPEAMENTO: UpdateTimelineEventDto → TimelineEvent
        // ==========================================
        // Usado quando recebemos dados do PUT e atualizamos uma entidade
        CreateMap<UpdateTimelineEventDto, TimelineEvent>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Não altera data de criação
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt será setado no handler
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TimelineEventType)src.Type));
    }
}