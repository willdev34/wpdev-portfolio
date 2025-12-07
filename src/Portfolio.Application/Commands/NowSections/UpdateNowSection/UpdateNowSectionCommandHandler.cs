// ====================================
// Título: UpdateNowSectionCommandHandler.cs
// Descrição: Handler que processa UpdateNowSectionCommand e atualiza a seção
// Autor: Will
// Empresa: WpDev
// Data: 06/12/2024
// ====================================

using AutoMapper;
using MediatR;
using Portfolio.Application.Interfaces;

namespace Portfolio.Application.Commands.NowSections.UpdateNowSection;

public class UpdateNowSectionCommandHandler : IRequestHandler<UpdateNowSectionCommand, Unit>
{
    private readonly INowSectionRepository _repository;
    private readonly IMapper _mapper;

    public UpdateNowSectionCommandHandler(
        INowSectionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        UpdateNowSectionCommand request, 
        CancellationToken cancellationToken)
    {
        // ====================================
        // 1. VERIFICAR SE A SEÇÃO EXISTE
        // ====================================
        var existingSection = await _repository.GetByIdAsync(request.SectionData.Id);

        if (existingSection == null)
        {
            throw new KeyNotFoundException($"Seção com ID {request.SectionData.Id} não encontrada");
        }

        // ====================================
        // 2. SE ESTÁ ATIVANDO ESTA SEÇÃO, DESATIVAR TODAS AS OUTRAS
        // ====================================
        // Business rule: Apenas 1 seção pode estar ativa por vez
        if (request.SectionData.IsActive && !existingSection.IsActive)
        {
            await _repository.DeactivateAllAsync();
        }

        // ====================================
        // 3. ATUALIZAR A SEÇÃO
        // ====================================
        var updatedSection = _mapper.Map(request.SectionData, existingSection);

        await _repository.UpdateAsync(updatedSection);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}