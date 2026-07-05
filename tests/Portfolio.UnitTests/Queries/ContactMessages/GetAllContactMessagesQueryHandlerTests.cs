// Título: GetAllContactMessagesQueryHandlerTests.cs
// Descrição: Testes unitários do handler de listagem de mensagens de contato

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Queries.ContactMessages.GetAllContactMessages;
using Portfolio.Domain.Entities;

namespace Portfolio.UnitTests.Queries.ContactMessages;

public class GetAllContactMessagesQueryHandlerTests
{
    private readonly Mock<IContactMessageRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllContactMessagesQueryHandler _handler;

    public GetAllContactMessagesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllContactMessagesQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeMensagens_QuandoExistiremMensagens()
    {
        // Arrange
        var mensagens = new List<ContactMessage>
        {
            new() { Id = Guid.NewGuid(), Name = "Maria Silva", Email = "maria@teste.com", Subject = "Oportunidade", Message = "Olá!" },
            new() { Id = Guid.NewGuid(), Name = "João Costa", Email = "joao@teste.com", Subject = "Parceria", Message = "Oi!" }
        };

        var dtos = new List<ContactMessageCardDto>
        {
            new() { Id = mensagens[0].Id, Name = "Maria Silva", Email = "maria@teste.com" },
            new() { Id = mensagens[1].Id, Name = "João Costa", Email = "joao@teste.com" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(mensagens);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ContactMessageCardDto>>(mensagens))
            .Returns(dtos);

        // Act
        var resultado = await _handler.Handle(new GetAllContactMessagesQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Name.Should().Be("Maria Silva");
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistiremMensagens()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ContactMessage>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ContactMessageCardDto>>(It.IsAny<IEnumerable<ContactMessage>>()))
            .Returns(new List<ContactMessageCardDto>());

        // Act
        var resultado = await _handler.Handle(new GetAllContactMessagesQuery(), CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_DeveChamarGetAllAsync_UmaVez()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ContactMessage>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ContactMessageCardDto>>(It.IsAny<IEnumerable<ContactMessage>>()))
            .Returns(new List<ContactMessageCardDto>());

        // Act
        await _handler.Handle(new GetAllContactMessagesQuery(), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
}