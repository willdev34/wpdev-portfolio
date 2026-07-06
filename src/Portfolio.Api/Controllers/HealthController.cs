// Título: HealthController.cs
// Descrição: Endpoint de health check para manter API e banco acordados

using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IProjectRepository _repository;

    public HealthController(IProjectRepository repository)
    {
        _repository = repository;
    }

    // GET: api/health
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        // Query leve só pra manter o banco acordado
        await _repository.GetAllAsync();
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}