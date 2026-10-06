// Título: ProjectFormMapperTests.cs
// Descrição: Testes de regressão do carregamento do formulário de projeto do admin: carregar, exibir e salvar sem
//            alterar nada mantém o objeto inteiro igual, para todos os status.

using FluentAssertions;
using Portfolio.Domain.Entities;
using Portfolio.Web.DTOs.Projects;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class ProjectFormMapperTests
{
    // Todos os campos com valores diferentes do padrão: se algum campo novo for esquecido no mapper, ele volta ao padrão e o teste falha
    private static ProjectDto Original(string status, bool isActive = true) => new()
    {
        Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
        Title = "Portal Registro de Marca",
        Description = "Descrição completa do projeto, com mais de dez caracteres.",
        ShortDescription = "Resumo curto",
        ImageUrl = "https://res.cloudinary.com/demo/image/upload/v1/capa.png",
        DemoUrl = "https://exemplo.com/demo",
        RepositoryUrl = "https://github.com/exemplo/repo",
        Technologies = new List<string> { "WordPress", "PHP", "CSS" },
        Year = 2017,
        IsFeatured = true,
        Status = status,
        CreatedAt = new DateTime(2026, 9, 21, 1, 50, 36, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2026, 9, 22, 8, 0, 0, DateTimeKind.Utc),
        IsActive = isActive
    };

    private static UpdateProjectDto EsperadoSemMudanca(ProjectDto o, int status) => new()
    {
        Id = o.Id,
        Title = o.Title,
        Description = o.Description,
        ShortDescription = o.ShortDescription,
        ImageUrl = o.ImageUrl,
        DemoUrl = o.DemoUrl,
        RepositoryUrl = o.RepositoryUrl,
        Technologies = o.Technologies.ToList(),
        Year = o.Year,
        IsFeatured = o.IsFeatured,
        Status = status,
        IsActive = o.IsActive
    };

    public static IEnumerable<object[]> TodosOsStatus() =>
        Enum.GetValues<ProjectStatus>().Select(s => new object[] { s.ToString(), (int)s });

    [Theory]
    [MemberData(nameof(TodosOsStatus))]
    public void IdaEVolta_SemEditar_DeveManterOObjetoInteiroIgual(string nomeDoStatus, int valorDoStatus)
    {
        var original = Original(nomeDoStatus);

        var carregado = ProjectFormMapper.ToForm(original);
        var salvo = ProjectFormMapper.ToUpdate(original, carregado.Form);

        carregado.UnknownStatus.Should().BeNull();
        salvo.Should().BeEquivalentTo(EsperadoSemMudanca(original, valorDoStatus), options => options.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(TodosOsStatus))]
    public void ToForm_DeveExibirOStatusCarregado_NaoOPadraoDoFormulario(string nomeDoStatus, int valorDoStatus)
    {
        // O bug original: o formulário abria sempre com Concluído (2), o padrão do DTO, em vez do status do projeto
        var resultado = ProjectFormMapper.ToForm(Original(nomeDoStatus));

        resultado.Form.Status.Should().Be(valorDoStatus);
        ProjectStatusHelper.IsValid(resultado.Form.Status).Should().BeTrue();
    }

    [Fact]
    public void ToForm_DeveCopiarTodosOsCamposEditaveis()
    {
        var original = Original("InProgress");

        var form = ProjectFormMapper.ToForm(original).Form;

        form.Should().BeEquivalentTo(new CreateProjectDto
        {
            Title = original.Title,
            Description = original.Description,
            ShortDescription = original.ShortDescription,
            ImageUrl = original.ImageUrl,
            DemoUrl = original.DemoUrl,
            RepositoryUrl = original.RepositoryUrl,
            Technologies = original.Technologies.ToList(),
            Year = original.Year,
            IsFeatured = original.IsFeatured,
            Status = 1
        });
    }

    [Fact]
    public void ToUpdate_NaoDeixaNenhumaPropriedadeNoValorPadrao_QuandoOProjetoTemTodosOsCampos()
    {
        // Proteção contra campo novo esquecido: toda propriedade do DTO de atualização precisa vir preenchida
        var original = Original("Archived");

        var salvo = ProjectFormMapper.ToUpdate(original, ProjectFormMapper.ToForm(original).Form);

        foreach (var propriedade in typeof(UpdateProjectDto).GetProperties())
        {
            var valor = propriedade.GetValue(salvo);
            var padrao = propriedade.PropertyType.IsValueType ? Activator.CreateInstance(propriedade.PropertyType) : null;
            valor.Should().NotBe(padrao, $"{propriedade.Name} deveria vir do projeto original");
            if (valor is System.Collections.IEnumerable lista && valor is not string)
            {
                lista.Cast<object>().Should().NotBeEmpty(propriedade.Name);
            }
        }
    }

    [Fact]
    public void ToForm_NaoDeveAlterarNemCompartilharAListaDeTecnologiasDoOriginal()
    {
        var original = Original("Completed");

        var form = ProjectFormMapper.ToForm(original).Form;
        form.Technologies.Add("Docker");

        original.Technologies.Should().Equal("WordPress", "PHP", "CSS");
    }

    [Fact]
    public void ToUpdate_DeveUsarOsValoresEditadosESoEles_PreservandoOResto()
    {
        var original = Original("InProgress");
        var form = ProjectFormMapper.ToForm(original).Form;

        form.Title = "Título novo";
        form.Status = 3;
        form.Technologies.Add("Docker");
        var salvo = ProjectFormMapper.ToUpdate(original, form);

        var esperado = EsperadoSemMudanca(original, 3);
        esperado.Title = "Título novo";
        esperado.Technologies = new List<string> { "WordPress", "PHP", "CSS", "Docker" };
        salvo.Should().BeEquivalentTo(esperado, options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToUpdate_DeveManterIsActiveDoProjetoOriginal_EmVezDeFixarVerdadeiro(bool isActive)
    {
        var original = Original("Completed", isActive);

        var salvo = ProjectFormMapper.ToUpdate(original, ProjectFormMapper.ToForm(original).Form);

        salvo.IsActive.Should().Be(isActive);
    }

    [Theory]
    [InlineData("Paused", "Paused")]
    [InlineData("99", "99")]
    [InlineData("", "(vazio)")]
    [InlineData("   ", "(vazio)")]
    public void ToForm_DeveSinalizarStatusDesconhecido_SemInventarUmStatusValido(string statusDaApi, string textoDoAviso)
    {
        var resultado = ProjectFormMapper.ToForm(Original(statusDaApi));

        resultado.UnknownStatus.Should().Be(textoDoAviso);
        resultado.Form.Status.Should().Be(ProjectStatusHelper.Unknown);
        ProjectStatusHelper.IsValid(resultado.Form.Status).Should().BeFalse();
    }

    [Fact]
    public void ToForm_ComStatusDesconhecido_DeveCarregarOsDemaisCamposNormalmente()
    {
        var original = Original("Paused");

        var form = ProjectFormMapper.ToForm(original).Form;

        form.Title.Should().Be(original.Title);
        form.Technologies.Should().Equal(original.Technologies);
        form.Year.Should().Be(original.Year);
    }
}
