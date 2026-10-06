// Título: TechnologyCleanupPlannerTests.cs
// Descrição: Testes do planejamento do script de limpeza de tecnologias (tools/DatabaseSetup), linkado no csproj

using DatabaseSetup;
using FluentAssertions;

namespace Portfolio.UnitTests.Tools;

public class TechnologyCleanupPlannerTests
{
    private static ProjectTechnologies Projeto(string titulo, params string[] tecnologias) =>
        new(Guid.NewGuid(), titulo, tecnologias);

    [Fact]
    public void Plan_DeveListarSoOsProjetosQueMudam()
    {
        var limpo = Projeto("Limpo", "React", "CSS", "JavaScript");
        var sujo = Projeto("Sujo", "Javascript", "CSS3", "CSS 3", "HTML 5", "React");

        var mudancas = TechnologyCleanupPlanner.Plan(new[] { limpo, sujo });

        mudancas.Should().ContainSingle();
        mudancas[0].Id.Should().Be(sujo.Id);
        mudancas[0].Before.Should().Equal("Javascript", "CSS3", "CSS 3", "HTML 5", "React");
        mudancas[0].After.Should().Equal("JavaScript", "CSS", "HTML", "React");
    }

    [Fact]
    public void Plan_DeveSerIdempotente_QuandoAplicadoSobreOResultado()
    {
        var sujo = Projeto("Sujo", "JS", "Github Pages", "  Docker ", "docker");
        var primeira = TechnologyCleanupPlanner.Plan(new[] { sujo });

        var aplicado = primeira.Select(c => new ProjectTechnologies(c.Id, c.Title, c.After));
        var segunda = TechnologyCleanupPlanner.Plan(aplicado);

        primeira.Should().ContainSingle();
        segunda.Should().BeEmpty();
    }

    [Fact]
    public void Plan_DeveDevolverVazio_QuandoNaoHaProjetos()
    {
        TechnologyCleanupPlanner.Plan(Array.Empty<ProjectTechnologies>()).Should().BeEmpty();
    }

    [Fact]
    public void Plan_DeveManterVersoes_ENaoMexerEmProjetoJaNormalizado()
    {
        var projeto = Projeto("Versões", "Next.js 16", "Django 4.2", "Prisma 7", "GitHub Actions");

        TechnologyCleanupPlanner.Plan(new[] { projeto }).Should().BeEmpty();
    }
}
