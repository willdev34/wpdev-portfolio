// Título: ContactMessageCommandHandlerTests.cs
// Descrição: Testes unitários dos commands de ContactMessages (Delete, Update)

using FluentAssertions;
using MediatR;
using Moq;
using Portfolio.Application.Commands.ContactMessages.DeleteContactMessage;
using Portfolio.Application.Commands.ContactMessages.UpdateContactMessage;
using Portfolio.Application.DTOs.ContactMessages;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using AutoMapper;

namespace Portfolio.UnitTests.ContactMessages;

public class DeleteContactMessageCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _repositoryMock;
    private readonly DeleteContactMessageCommandHandler _handler;

    public DeleteContactMessageCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContactMessageRepository>();
        _handler = new DeleteContactMessageCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveDeletarMensagem_QuandoMensagemExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(new DeleteContactMessageCommand(id), CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoMensagemNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(new DeleteContactMessageCommand(id), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NaoDeveChamarDelete_QuandoMensagemNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

        // Act
        try { await _handler.Handle(new DeleteContactMessageCommand(id), CancellationToken.None); } catch { }

        // Assert
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}

public class UpdateContactMessageCommandHandlerTests
{
    private readonly Mock<IContactMessageRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateContactMessageCommandHandler _handler;

    public UpdateContactMessageCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateContactMessageCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAtualizarMensagem_QuandoMensagemExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mensagem = new ContactMessage { Id = id, Name = "Maria", Status = ContactMessageStatus.New };
        var updateDto = new UpdateContactMessageDto { Id = id, Status = 1 };
        var command = new UpdateContactMessageCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mensagem);
        _repositoryMock.Setup(r => r.UpdateAsync(mensagem)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        _repositoryMock.Verify(r => r.UpdateAsync(mensagem), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoMensagemNaoExistir()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateContactMessageDto { Id = id, Status = 1 };
        var command = new UpdateContactMessageCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ContactMessage?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_DeveSetarRespondedAt_QuandoResponseMessageForFornecido()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mensagem = new ContactMessage 
        { 
            Id = id, 
            Name = "Maria", 
            Status = ContactMessageStatus.New,
            RespondedAt = null
        };
        var updateDto = new UpdateContactMessageDto 
        { 
            Id = id, 
            Status = 1,
            ResponseMessage = "Obrigado pelo contato!"
        };
        var command = new UpdateContactMessageCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mensagem);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ContactMessage>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        mensagem.RespondedAt.Should().NotBeNull();
        mensagem.Status.Should().Be(ContactMessageStatus.Responded);
        mensagem.ResponseMessage.Should().Be("Obrigado pelo contato!");
    }

    [Fact]
    public async Task Handle_NaoDeveAtualizarRespondedAt_QuandoJaFoiRespondida()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dataAnterior = DateTime.UtcNow.AddDays(-1);
        var mensagem = new ContactMessage 
        { 
            Id = id, 
            Name = "Maria", 
            Status = ContactMessageStatus.Responded,
            RespondedAt = dataAnterior
        };
        var updateDto = new UpdateContactMessageDto 
        { 
            Id = id, 
            Status = 2,
            ResponseMessage = "Nova resposta"
        };
        var command = new UpdateContactMessageCommand(updateDto);

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mensagem);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ContactMessage>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        mensagem.RespondedAt.Should().Be(dataAnterior);
    }
}