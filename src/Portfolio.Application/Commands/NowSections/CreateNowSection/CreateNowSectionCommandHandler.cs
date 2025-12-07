// ====================================
// Título: CreateNowSectionCommandHandler.cs
// Descrição: Handler que processa CreateNowSectionCommand e cria a seção no banco
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.DTOs.NowSections;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Commands.NowSections.CreateNowSection;

/// <summary>
/// Handler responsável por processar o command CreateNowSectionCommand
/// 1. DESATIVA todas as seções anteriores (business rule)
/// 2. Converte DTO para Entity
/// 3. Salva no banco via Repository
/// 4. Retorna o NowSectionDto da seção criada
/// </summary>
public class CreateNowSectionCommandHandler : IRequestHandler<CreateNowSectionCommand, NowSectionDto>
{
    private readonly INowSectionRepository _repository;
    private readonly IMapper _mapper;

    // ====================================
    // CONSTRUTOR - Injeção de Dependência
    // ====================================
    public CreateNowSectionCommandHandler(
        INowSectionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ====================================
    // HANDLE - Processa o Command
    // ====================================
    /// <summary>
    /// Método principal que executa a lógica do command
    /// </summary>
    public async Task<NowSectionDto> Handle(
        CreateNowSectionCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. DESATIVAR TODAS AS SEÇÕES ANTERIORES
        // ====================================
        // IMPORTANTE: Apenas 1 seção pode estar ativa por vez
        await _repository.DeactivateAllAsync();

        // ====================================
        // 2. CONVERTER DTO → ENTITY
        // ====================================
        var nowSection = _mapper.Map<NowSection>(request.SectionData);

        // ====================================
        // 3. SALVAR NO BANCO
        // ====================================
        var createdSection = await _repository.AddAsync(nowSection);
        await _repository.SaveChangesAsync();

        // ====================================
        // 4. CONVERTER ENTITY → DTO E RETORNAR
        // ====================================
        var nowSectionDto = _mapper.Map<NowSectionDto>(createdSection);

        return nowSectionDto;
    }
}