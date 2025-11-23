// ====================================
// Título: ProjectMappingProfile.cs (REFATORADO)
// Descrição: Profile do AutoMapper para a entidade Project
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using AutoMapper;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings;

/// <summary>
/// Profile do AutoMapper para a entidade Project
/// Define como converter Project (Entity) ↔ DTOs
/// </summary>
public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        // ==========================================
        // MAPEAMENTO: Project → ProjectDto
        // ==========================================
        // Usado quando buscamos do banco e retornamos na API
        // Converte o enum Status para string
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // ==========================================
        // MAPEAMENTO: Project → ProjectCardDto
        // ==========================================
        // Usado para listagens (grid de cards/blocos editoriais)
        CreateMap<Project, ProjectCardDto>();

        // ==========================================
        // MAPEAMENTO: CreateProjectDto → Project
        // ==========================================
        // Usado quando recebemos dados do POST e criamos uma entidade
        CreateMap<CreateProjectDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id será gerado automaticamente
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt será setado no handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Novo projeto sempre ativo
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ProjectStatus)src.Status));

        // ==========================================
        // MAPEAMENTO: UpdateProjectDto → Project
        // ==========================================
        // Usado quando recebemos dados do PUT e atualizamos uma entidade
        CreateMap<UpdateProjectDto, Project>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Não altera data de criação
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt será setado no handler
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ProjectStatus)src.Status));
    }
}