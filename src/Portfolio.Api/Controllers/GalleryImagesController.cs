// ====================================
// Título: GalleryImagesController.cs
// Descrição: Controller REST API para gerenciar imagens da galeria
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Commands.GalleryImages.CreateGalleryImage;
using Portfolio.Application.Commands.GalleryImages.UpdateGalleryImage;
using Portfolio.Application.Commands.GalleryImages.DeleteGalleryImage;
using Portfolio.Application.DTOs.GalleryImages;
using Portfolio.Application.Queries.GalleryImages.GetAllGalleryImages;
using Portfolio.Application.Queries.GalleryImages.GetGalleryImageById;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Controller responsável pelos endpoints de GalleryImages
/// Todos os endpoints usam CQRS via MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GalleryImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public GalleryImagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ====================================
    // GET: api/galleryimages
    // ====================================
    /// <summary>
    /// Busca TODAS as imagens da galeria
    /// Ordenadas por Order (crescente)
    /// </summary>
    /// <returns>Lista de GalleryImageCardDto</returns>
    /// <response code="200">Retorna a lista de imagens</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GalleryImageCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GalleryImageCardDto>>> GetAll()
    {
        // Cria a query
        var query = new GetAllGalleryImagesQuery();
        
        // Envia para o MediatR processar
        var images = await _mediator.Send(query);
        
        // Retorna HTTP 200 OK com a lista
        return Ok(images);
    }

    // ====================================
    // GET: api/galleryimages/{id}
    // ====================================
    /// <summary>
    /// Busca uma imagem específica por ID
    /// </summary>
    /// <param name="id">ID da imagem</param>
    /// <returns>GalleryImageDto completo</returns>
    /// <response code="200">Retorna a imagem encontrada</response>
    /// <response code="404">Imagem não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GalleryImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GalleryImageDto>> GetById(Guid id)
    {
        // Cria a query
        var query = new GetGalleryImageByIdQuery(id);
        
        // Envia para o MediatR processar
        var galleryImage = await _mediator.Send(query);
        
        // Se não encontrou, retorna 404
        if (galleryImage == null)
        {
            return NotFound(new { message = $"Imagem com ID {id} não encontrada" });
        }
        
        // Retorna HTTP 200 OK com a imagem
        return Ok(galleryImage);
    }

    // ====================================
    // POST: api/galleryimages
    // ====================================
    /// <summary>
    /// Adiciona uma nova imagem à galeria
    /// </summary>
    /// <param name="createDto">Dados da imagem a ser criada</param>
    /// <returns>GalleryImageDto da imagem criada</returns>
    /// <response code="201">Imagem criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(GalleryImageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GalleryImageDto>> Create([FromBody] CreateGalleryImageDto createDto)
    {
        // Valida se o DTO veio preenchido
        if (createDto == null)
        {
            return BadRequest(new { message = "Dados da imagem são obrigatórios" });
        }

        // Cria o command
        var command = new CreateGalleryImageCommand(createDto);
        
        // Envia para o MediatR processar
        // O Validator será executado automaticamente aqui
        var createdImage = await _mediator.Send(command);
        
        // Retorna HTTP 201 Created com a imagem criada
        // O header Location apontará para GET /api/galleryimages/{id}
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdImage.Id }, 
            createdImage
        );
    }

    // ====================================
    // PUT: api/galleryimages/{id}
    // ====================================
    /// <summary>
    /// Atualiza uma imagem existente
    /// </summary>
    /// <param name="id">ID da imagem</param>
    /// <param name="updateDto">Dados atualizados</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Imagem atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Imagem não encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGalleryImageDto updateDto)
    {
        // Valida se o ID da URL bate com o ID do body
        if (id != updateDto.Id)
        {
            return BadRequest(new { message = "O ID da URL não corresponde ao ID do body" });
        }

        try
        {
            // Cria o command
            var command = new UpdateGalleryImageCommand(updateDto);
            
            // Envia para o MediatR processar
            await _mediator.Send(command);
            
            // Retorna HTTP 204 No Content (sucesso, sem corpo)
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ====================================
    // DELETE: api/galleryimages/{id}
    // ====================================
    /// <summary>
    /// Deleta uma imagem (SOFT DELETE - marca como invisível)
    /// A imagem pode ser recuperada depois
    /// </summary>
    /// <param name="id">ID da imagem</param>
    /// <returns>NoContent se sucesso</returns>
    /// <response code="204">Imagem deletada com sucesso</response>
    /// <response code="404">Imagem não encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            // Cria o command
            var command = new DeleteGalleryImageCommand(id);
            
            // Envia para o MediatR processar
            await _mediator.Send(command);
            
            // Retorna HTTP 204 No Content (sucesso)
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}