// ====================================
// Título: BlogPostMappingProfile.cs
// Descrição: Profile do AutoMapper para a entidade BlogPost
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade BlogPost
/// Define como converter BlogPost (Entity) ↔ DTOs
/// </summary>
public class BlogPostMappingProfile : Profile
{
    public BlogPostMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: BlogPost → BlogPostDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API
        // Status (enum) é convertido para string
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ==========================================
        // MAPEAMENTO: BlogPost → BlogPostCardDto
        // ==========================================
        // Usado para listagens públicas (grid de cards editoriais)
        // NÃO inclui Status nem ScheduledAt - DTO público
        CreateMap<BlogPost, BlogPostCardDto>();

        // ==========================================
        // MAPEAMENTO: BlogPost → BlogPostAdminCardDto
        // ==========================================
        // Usado na listagem do ADMIN. Status (enum) é convertido para string
        CreateMap<BlogPost, BlogPostAdminCardDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ==========================================
        // MAPEAMENTO: CreateBlogPostDto → BlogPost
        // ==========================================
        // Usado quando recebemos dados do POST e criamos uma entidade
        CreateMap<CreateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => 0)) // Novo post sempre começa com 0 views
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Status é sincronizado no handler
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore()) // PublishedAt é sincronizado no handler
            .ForMember(dest => dest.ScheduledAt, opt => opt.Ignore()) // ScheduledAt é sincronizado no handler
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore()); // Será setado futuramente quando tivermos autenticação

        // ==========================================
        // MAPEAMENTO: UpdateBlogPostDto → BlogPost
        // ==========================================
        // Usado quando recebemos dados do PUT e atualizamos uma entidade
        CreateMap<UpdateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Não altera data de criação
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt será setado no handler
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore()) // ViewCount não deve ser alterado manualmente
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Status é sincronizado no handler
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore()) // PublishedAt é sincronizado no handler
            .ForMember(dest => dest.ScheduledAt, opt => opt.Ignore()) // ScheduledAt é sincronizado no handler
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore()); // Não altera autor
    }
}