using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.Projects;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Queries.Projects.GetFeaturedProjects;

public class GetFeaturedProjectsQueryHandler
    : IRequestHandler<GetFeaturedProjectsQuery, IEnumerable<ProjectCardDto>>
{
    private readonly IProjectRepository _repository;
    private readonly IMapper _mapper;

    public GetFeaturedProjectsQueryHandler(
        IProjectRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectCardDto>> Handle(
        GetFeaturedProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _repository.GetFeaturedAsync();
        return _mapper.Map<IEnumerable<ProjectCardDto>>(projects);
    }
}