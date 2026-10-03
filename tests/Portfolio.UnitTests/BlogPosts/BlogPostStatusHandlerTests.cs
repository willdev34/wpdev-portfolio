// Título: BlogPostStatusHandlerTests.cs
// Descrição: Testes unitários da sincronização de Status, IsPublished, PublishedAt e ScheduledAt
//            nos handlers de Create e Update de BlogPosts

using AutoMapper;
using FluentAssertions;
using Moq;
using Portfolio.Application.Commands.BlogPosts.CreateBlogPost;
using Portfolio.Application.Commands.BlogPosts.UpdateBlogPost;
using Portfolio.Application.DTOs.BlogPosts;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.UnitTests.BlogPosts;

public class CreateBlogPostCommandHandlerStatusTests
{
    // Margem folgada: o handler usa DateTime.UtcNow direto, sem IClock
    private static readonly TimeSpan Margem = TimeSpan.FromDays(1);

    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateBlogPostCommandHandler _handler;

    public CreateBlogPostCommandHandlerStatusTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateBlogPostCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveVirarPublished_QuandoStatusNuloEIsPublishedTrue()
    {
        // Arrange
        var dto = BuildDto(status: null, isPublished: true);
        var entity = SetupMapperAndRepository(dto);

        // Act
        await _handler.Handle(new CreateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        entity.Status.Should().Be(BlogPostStatus.Published);
        entity.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeveVirarDraft_QuandoStatusNuloEIsPublishedFalse()
    {
        // Arrange
        var dto = BuildDto(status: null, isPublished: false);
        var entity = SetupMapperAndRepository(dto);

        // Act
        await _handler.Handle(new CreateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        entity.Status.Should().Be(BlogPostStatus.Draft);
        entity.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DevePublicarAgora_QuandoPublishedEPublishedAtNulo()
    {
        // Arrange
        var dto = BuildDto(BlogPostStatus.Published, scheduledAt: DateTime.UtcNow.AddDays(5));
        var entity = SetupMapperAndRepository(dto);

        // Act
        await _handler.Handle(new CreateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        entity.IsPublished.Should().BeTrue();
        entity.ScheduledAt.Should().BeNull();
        entity.PublishedAt.Should().NotBeNull();
        entity.PublishedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, Margem);
    }

    [Fact]
    public async Task Handle_DeveAgendar_QuandoScheduled()
    {
        // Arrange
        var agendamento = DateTime.UtcNow.AddDays(10);
        var dto = BuildDto(BlogPostStatus.Scheduled, scheduledAt: agendamento);
        var entity = SetupMapperAndRepository(dto);

        // Act
        await _handler.Handle(new CreateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        entity.Status.Should().Be(BlogPostStatus.Scheduled);
        entity.IsPublished.Should().BeFalse();
        entity.ScheduledAt.Should().Be(agendamento);
        entity.PublishedAt.Should().Be(agendamento);
    }

    [Fact]
    public async Task Handle_DeveSalvarRascunho_QuandoDraft()
    {
        // Arrange
        var dto = BuildDto(BlogPostStatus.Draft, scheduledAt: DateTime.UtcNow.AddDays(5));
        var entity = SetupMapperAndRepository(dto);

        // Act
        await _handler.Handle(new CreateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        entity.Status.Should().Be(BlogPostStatus.Draft);
        entity.IsPublished.Should().BeFalse();
        entity.ScheduledAt.Should().BeNull();
        entity.PublishedAt.Should().BeNull();
    }

    private BlogPost SetupMapperAndRepository(CreateBlogPostDto dto)
    {
        // O AutoMapper real ignora Status, PublishedAt e ScheduledAt (sincronizados no handler)
        var entity = new BlogPost { Id = Guid.NewGuid(), Title = dto.Title, Slug = dto.Slug };

        _repositoryMock.Setup(r => r.SlugExistsAsync(dto.Slug, null)).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<BlogPost>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<BlogPostDto>(entity)).Returns(new BlogPostDto { Id = entity.Id });

        return entity;
    }

    private static CreateBlogPostDto BuildDto(
        BlogPostStatus? status,
        bool isPublished = false,
        DateTime? scheduledAt = null) => new()
    {
        Title = "Post de Status",
        Slug = "post-de-status",
        Status = status,
        IsPublished = isPublished,
        ScheduledAt = scheduledAt
    };
}

public class UpdateBlogPostCommandHandlerStatusTests
{
    // Margem folgada: o handler usa DateTime.UtcNow direto, sem IClock
    private static readonly TimeSpan Margem = TimeSpan.FromDays(1);

    private readonly Mock<IBlogPostRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateBlogPostCommandHandler _handler;

    public UpdateBlogPostCommandHandlerStatusTests()
    {
        _repositoryMock = new Mock<IBlogPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateBlogPostCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveVirarPublished_QuandoStatusNuloEIsPublishedTrue()
    {
        // Arrange
        var existing = BuildExisting(BlogPostStatus.Draft, publishedAt: null);
        var dto = BuildDto(existing.Id, status: null, isPublished: true);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Published);
        existing.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeveVirarDraft_QuandoStatusNuloEIsPublishedFalse()
    {
        // Arrange
        var existing = BuildExisting(BlogPostStatus.Published, publishedAt: DateTime.UtcNow.AddDays(-10));
        var dto = BuildDto(existing.Id, status: null, isPublished: false);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Draft);
        existing.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DevePublicarAgora_QuandoPublishedEPublishedAtNulo()
    {
        // Arrange
        var existing = BuildExisting(BlogPostStatus.Draft, publishedAt: null);
        var dto = BuildDto(existing.Id, BlogPostStatus.Published);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.IsPublished.Should().BeTrue();
        existing.ScheduledAt.Should().BeNull();
        existing.PublishedAt.Should().NotBeNull();
        existing.PublishedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, Margem);
    }

    [Fact]
    public async Task Handle_DeveSubstituirPublishedAtFuturoPorAgora_QuandoPublishedVindoDeAgendado()
    {
        // Arrange
        var agendamentoFuturo = DateTime.UtcNow.AddDays(10);
        var existing = BuildExisting(BlogPostStatus.Scheduled, publishedAt: agendamentoFuturo, scheduledAt: agendamentoFuturo);
        var dto = BuildDto(existing.Id, BlogPostStatus.Published);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Published);
        existing.IsPublished.Should().BeTrue();
        existing.ScheduledAt.Should().BeNull();
        existing.PublishedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, Margem);
    }

    [Fact]
    public async Task Handle_DeveManterPublishedAt_QuandoPublishedComDataNoPassado()
    {
        // Arrange
        var dataOriginal = DateTime.UtcNow.AddDays(-10);
        var existing = BuildExisting(BlogPostStatus.Draft, publishedAt: dataOriginal);
        var dto = BuildDto(existing.Id, BlogPostStatus.Published);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.IsPublished.Should().BeTrue();
        existing.PublishedAt.Should().Be(dataOriginal);
    }

    [Fact]
    public async Task Handle_DeveAgendar_QuandoScheduled()
    {
        // Arrange
        var agendamento = DateTime.UtcNow.AddDays(10);
        var existing = BuildExisting(BlogPostStatus.Draft, publishedAt: null);
        var dto = BuildDto(existing.Id, BlogPostStatus.Scheduled, scheduledAt: agendamento);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Scheduled);
        existing.IsPublished.Should().BeFalse();
        existing.ScheduledAt.Should().Be(agendamento);
        existing.PublishedAt.Should().Be(agendamento);
    }

    [Fact]
    public async Task Handle_DeveVirarRascunhoPreservandoPublishedAt_QuandoDraft()
    {
        // Arrange
        var dataOriginal = DateTime.UtcNow.AddDays(-10);
        var existing = BuildExisting(BlogPostStatus.Published, publishedAt: dataOriginal);
        var dto = BuildDto(existing.Id, BlogPostStatus.Draft, scheduledAt: DateTime.UtcNow.AddDays(5));
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Draft);
        existing.IsPublished.Should().BeFalse();
        existing.ScheduledAt.Should().BeNull();
        existing.PublishedAt.Should().Be(dataOriginal);
    }

    // ====================================
    // CASO DE BORDA: post Scheduled com ScheduledAt já vencido
    // ====================================
    // Documenta o comportamento atual, sem corrigir produção.
    // Na API real o validator barra Status=Scheduled com data no passado (400) antes de chegar aqui.

    [Fact]
    public async Task Handle_DeveGravarComoScheduledNaoPublicadoMasPublico_QuandoScheduledComDataVencida()
    {
        // Arrange
        var agendamentoVencido = DateTime.UtcNow.AddDays(-2);
        var existing = BuildExisting(BlogPostStatus.Scheduled, publishedAt: agendamentoVencido, scheduledAt: agendamentoVencido);
        var dto = BuildDto(existing.Id, BlogPostStatus.Scheduled, scheduledAt: agendamentoVencido);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Scheduled);
        existing.ScheduledAt.Should().Be(agendamentoVencido);
        existing.PublishedAt.Should().Be(agendamentoVencido);
        existing.IsPublished.Should().BeFalse();
        // IsPublic usa a regra de Status, então o post continua visível mesmo com IsPublished false
        existing.IsPublic.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeveManterDataDoAgendamento_QuandoScheduledVencidoViraPublished()
    {
        // Arrange
        var agendamentoVencido = DateTime.UtcNow.AddDays(-2);
        var existing = BuildExisting(BlogPostStatus.Scheduled, publishedAt: agendamentoVencido, scheduledAt: agendamentoVencido);
        var dto = BuildDto(existing.Id, BlogPostStatus.Published);
        SetupMapperAndRepository(dto, existing);

        // Act
        await _handler.Handle(new UpdateBlogPostCommand(dto), CancellationToken.None);

        // Assert
        existing.Status.Should().Be(BlogPostStatus.Published);
        existing.IsPublished.Should().BeTrue();
        existing.ScheduledAt.Should().BeNull();
        existing.PublishedAt.Should().Be(agendamentoVencido);
    }

    [Fact]
    public void Validator_DeveBarrarUpdate_QuandoScheduledComDataVencida()
    {
        // Arrange
        var agendamentoVencido = DateTime.UtcNow.AddDays(-2);
        var dto = BuildDto(Guid.NewGuid(), BlogPostStatus.Scheduled, scheduledAt: agendamentoVencido);
        dto.Title = "Post agendado vencido";
        dto.Excerpt = "Resumo com mais de dez caracteres.";
        dto.Content = new string('x', 60);
        dto.ReadTimeMinutes = 5;
        var validator = new UpdateBlogPostCommandValidator();

        // Act
        var resultado = validator.Validate(new UpdateBlogPostCommand(dto));

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.ErrorMessage == "A data de agendamento deve ser no futuro");
    }

    private void SetupMapperAndRepository(UpdateBlogPostDto dto, BlogPost existing)
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.SlugExistsAsync(dto.Slug, existing.Id)).ReturnsAsync(false);
        // O AutoMapper real devolve o próprio existing em Map(dto, existing)
        _mapperMock.Setup(m => m.Map(dto, existing)).Returns(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }

    private static BlogPost BuildExisting(
        BlogPostStatus status,
        DateTime? publishedAt,
        DateTime? scheduledAt = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Post existente",
        Slug = "post-existente",
        Status = status,
        IsPublished = status == BlogPostStatus.Published,
        PublishedAt = publishedAt,
        ScheduledAt = scheduledAt
    };

    private static UpdateBlogPostDto BuildDto(
        Guid id,
        BlogPostStatus? status,
        bool isPublished = false,
        DateTime? scheduledAt = null) => new()
    {
        Id = id,
        Title = "Post existente",
        Slug = "post-existente",
        Status = status,
        IsPublished = isPublished,
        ScheduledAt = scheduledAt
    };
}
