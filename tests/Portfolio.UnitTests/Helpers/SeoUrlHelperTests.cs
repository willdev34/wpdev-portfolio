// Título: SeoUrlHelperTests.cs
// Descrição: Testes unitários do SeoUrlHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class SeoUrlHelperTests
{
    [Fact]
    public void BaseUrl_DeveSerODominioCanonicoComWww()
    {
        SeoUrlHelper.BaseUrl.Should().Be("https://www.wpdevbr.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("/")]
    public void BuildFullUrl_DeveDevolverARaiz_QuandoCaminhoVazioOuRaiz(string? path)
    {
        // Act
        var url = SeoUrlHelper.BuildFullUrl(path);

        // Assert
        url.Should().Be("https://www.wpdevbr.com/");
    }

    [Theory]
    [InlineData("/projects", "https://www.wpdevbr.com/projects")]
    [InlineData("projects", "https://www.wpdevbr.com/projects")]
    [InlineData("/blog/meu-post", "https://www.wpdevbr.com/blog/meu-post")]
    public void BuildFullUrl_DeveJuntarDominioECaminho_ComOuSemBarraInicial(string path, string esperado)
    {
        // Act
        var url = SeoUrlHelper.BuildFullUrl(path);

        // Assert
        url.Should().Be(esperado);
    }
}
