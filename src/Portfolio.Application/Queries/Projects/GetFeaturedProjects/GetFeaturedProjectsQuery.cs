using MediatR;
using Portfolio.Application.DTOs.Projects;

namespace Portfolio.Application.Queries.Projects.GetFeaturedProjects;

public record GetFeaturedProjectsQuery : IRequest<IEnumerable<ProjectCardDto>>;