// ====================================
// Título: TimelineEventRepositoryOrderingTests.cs
// Descrição: Testes de integração da ordenação cronológica do TimelineEventRepository contra o
//            PostgreSQL do Testcontainers: data inicial, desempate por data final, evento sem data
//            final (em andamento) depois dos que terminaram, e CreatedAt como último desempate.
//
//            Cada teste cria os próprios eventos (título único), inseridos pelo DbContext para
//            controlar CreatedAt, e compara a ordem relativa só entre eles. As datas ficam no
//            século 19, longe dos eventos reais e dos de outros testes.
// ====================================

using Microsoft.Extensions.DependencyInjection;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.IntegrationTests;

[Collection("Integration Tests")]
public class TimelineEventRepositoryOrderingTests : IntegrationTestBase
{
    private readonly CustomWebApplicationFactory _factory;

    public TimelineEventRepositoryOrderingTests(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllAsync_OrdersByStartDateAscending()
    {
        var unique = NewUnique();
        var jun = BuildEvent($"{unique}-jun", Utc(1851, 6, 1));
        var jan = BuildEvent($"{unique}-jan", Utc(1851, 1, 1));
        var dez = BuildEvent($"{unique}-dez", Utc(1851, 12, 1));
        // Insere fora de ordem para não depender da ordem de inserção
        await SeedAsync(jun, dez, jan);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(jan.Title, jun.Title, dez.Title);
    }

    [Fact]
    public async Task GetAllAsync_SameStartDate_OrdersByEndDateAscending()
    {
        var unique = NewUnique();
        var start = Utc(1852, 3, 1);
        var endsLate = BuildEvent($"{unique}-fim-tarde", start, endDate: Utc(1852, 12, 1));
        var endsEarly = BuildEvent($"{unique}-fim-cedo", start, endDate: Utc(1852, 5, 1));
        var endsMid = BuildEvent($"{unique}-fim-meio", start, endDate: Utc(1852, 8, 1));
        await SeedAsync(endsLate, endsEarly, endsMid);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(endsEarly.Title, endsMid.Title, endsLate.Title);
    }

    [Fact]
    public async Task GetAllAsync_SameStartDate_EventWithoutEndDateComesAfterTheOnesThatEnded()
    {
        // Sem data final = em andamento: vem depois de qualquer evento que já terminou
        var unique = NewUnique();
        var start = Utc(1853, 3, 1);
        var ongoing = BuildEvent($"{unique}-andamento", start, endDate: null, isCurrent: true);
        var endedFarFuture = BuildEvent($"{unique}-terminou-longe", start, endDate: Utc(1899, 12, 31));
        var endedSoon = BuildEvent($"{unique}-terminou-logo", start, endDate: Utc(1853, 4, 1));
        await SeedAsync(ongoing, endedFarFuture, endedSoon);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(endedSoon.Title, endedFarFuture.Title, ongoing.Title);
    }

    [Fact]
    public async Task GetAllAsync_SameStartAndEndDate_OrdersByCreatedAtAscending()
    {
        var unique = NewUnique();
        var start = Utc(1854, 3, 1);
        var end = Utc(1854, 9, 1);
        var createdFirst = BuildEvent($"{unique}-a", start, end, createdAt: Utc(2020, 1, 1));
        var createdSecond = BuildEvent($"{unique}-b", start, end, createdAt: Utc(2020, 1, 2));
        var createdThird = BuildEvent($"{unique}-c", start, end, createdAt: Utc(2020, 1, 3));
        await SeedAsync(createdThird, createdFirst, createdSecond);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(createdFirst.Title, createdSecond.Title, createdThird.Title);
    }

    [Fact]
    public async Task GetAllAsync_BothWithoutEndDateAndSameStartDate_OrdersByCreatedAtAscending()
    {
        var unique = NewUnique();
        var start = Utc(1855, 3, 1);
        var createdLater = BuildEvent($"{unique}-depois", start, endDate: null, createdAt: Utc(2021, 5, 2));
        var createdEarlier = BuildEvent($"{unique}-antes", start, endDate: null, createdAt: Utc(2021, 5, 1));
        await SeedAsync(createdLater, createdEarlier);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(createdEarlier.Title, createdLater.Title);
    }

    [Fact]
    public async Task GetAllAsync_CombinesAllCriteriaInTheDocumentedOrder()
    {
        var unique = NewUnique();
        // 1. data inicial; 2. data final (sem data final por último); 3. CreatedAt
        var e1 = BuildEvent($"{unique}-1", Utc(1856, 1, 1), Utc(1856, 2, 1), createdAt: Utc(2020, 1, 1));
        var e2 = BuildEvent($"{unique}-2", Utc(1856, 1, 1), Utc(1856, 2, 1), createdAt: Utc(2020, 1, 2));
        var e3 = BuildEvent($"{unique}-3", Utc(1856, 1, 1), Utc(1856, 6, 1));
        var e4 = BuildEvent($"{unique}-4", Utc(1856, 1, 1), endDate: null, createdAt: Utc(2020, 1, 1));
        var e5 = BuildEvent($"{unique}-5", Utc(1856, 1, 1), endDate: null, createdAt: Utc(2020, 1, 2));
        var e6 = BuildEvent($"{unique}-6", Utc(1857, 1, 1), endDate: null);
        await SeedAsync(e6, e4, e2, e5, e3, e1);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(e1.Title, e2.Title, e3.Title, e4.Title, e5.Title, e6.Title);
    }

    [Fact]
    public async Task GetAllAsync_IgnoresTheOrderField()
    {
        // O campo Order está em desuso: um Order alto não muda a posição
        var unique = NewUnique();
        var early = BuildEvent($"{unique}-cedo", Utc(1858, 1, 1), order: 999);
        var late = BuildEvent($"{unique}-tarde", Utc(1858, 6, 1), order: 0);
        await SeedAsync(late, early);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(early.Title, late.Title);
    }

    [Fact]
    public async Task GetAllAsync_ExcludesHiddenEvents()
    {
        var unique = NewUnique();
        var visible = BuildEvent($"{unique}-visivel", Utc(1859, 1, 1));
        var hidden = BuildEvent($"{unique}-oculto", Utc(1859, 2, 1), isVisible: false);
        await SeedAsync(visible, hidden);

        var ours = await GetOursAsync(unique);

        ours.Should().Equal(visible.Title);
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private async Task<List<string>> GetOursAsync(string unique)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        var repository = new TimelineEventRepository(context);

        var all = await repository.GetAllAsync();

        return all.Select(e => e.Title).Where(t => t.Contains(unique)).ToList();
    }

    private async Task SeedAsync(params TimelineEvent[] events)
    {
        // Insere direto pelo DbContext: o AddAsync do repositório sobrescreve CreatedAt
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        context.TimelineEvents.AddRange(events);
        await context.SaveChangesAsync();
    }

    private static string NewUnique() => Guid.NewGuid().ToString("N")[..8];

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    private static TimelineEvent BuildEvent(
        string title,
        DateTime date,
        DateTime? endDate = null,
        bool isCurrent = false,
        DateTime? createdAt = null,
        int order = 0,
        bool isVisible = true) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Description = "Descrição gerada pelo teste de ordenação da timeline.",
        Date = date,
        EndDate = endDate,
        IsCurrent = isCurrent,
        Type = TimelineEventType.Work,
        Order = order,
        IsVisible = isVisible,
        CreatedAt = createdAt ?? DateTime.UtcNow
    };
}
