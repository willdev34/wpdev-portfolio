// ====================================
// Título: ContactMessageMappingProfile.cs
// Descrição: Profile do AutoMapper para a entidade ContactMessage
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade ContactMessage
/// Define como converter ContactMessage (Entity) ↔ DTOs
/// Faz conversão de enums para string/int
/// </summary>
public class ContactMessageMappingProfile : Profile
{
    public ContactMessageMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: ContactMessage → ContactMessageDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API (ADMIN)
        // Converte os enums Type e Status para string
        CreateMap<ContactMessage, ContactMessageDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ==========================================
        // MAPEAMENTO: ContactMessage → ContactMessageCardDto
        // ==========================================
        // Usado para listagens no admin
        // Converte os enums Type e Status para string
        CreateMap<ContactMessage, ContactMessageCardDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ==========================================
        // MAPEAMENTO: CreateContactMessageDto → ContactMessage
        // ==========================================
        // Usado quando recebemos dados do POST (formulário público)
        // Converte int do DTO para enum da Entity
        CreateMap<CreateContactMessageDto, ContactMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ContactMessageStatus.New)) // Sempre começa como New
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore()) // Será setado no handler
            .ForMember(dest => dest.UserAgent, opt => opt.Ignore()) // Será setado no handler
            .ForMember(dest => dest.ResponseMessage, opt => opt.Ignore())
            .ForMember(dest => dest.RespondedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no repository
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (ContactMessageType)src.Type));

        // ==========================================
        // MAPEAMENTO: UpdateContactMessageDto → ContactMessage
        // ==========================================
        // Usado quando o ADMIN atualiza status/resposta (PUT)
        // Converte int do DTO para enum da Entity
        CreateMap<UpdateContactMessageDto, ContactMessage>()
            .ForMember(dest => dest.Name, opt => opt.Ignore()) // Não altera dados do remetente
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.Subject, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
            .ForMember(dest => dest.UserAgent, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt será setado no repository
            .ForMember(dest => dest.RespondedAt, opt => opt.Ignore()) // RespondedAt será setado no handler
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ContactMessageStatus)src.Status));
    }
}