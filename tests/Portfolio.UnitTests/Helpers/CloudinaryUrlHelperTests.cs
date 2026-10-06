// Título: CloudinaryUrlHelperTests.cs
// Descrição: Testes unitários do CloudinaryUrlHelper (Portfolio.Web), linkado no csproj sem referenciar o Web.

using FluentAssertions;
using Portfolio.Web.Helpers;

namespace Portfolio.UnitTests.Helpers;

public class CloudinaryUrlHelperTests
{
    private const string Url = "https://res.cloudinary.com/demo/image/upload/v1/foto.png";

    [Fact]
    public void Optimize_DeveInserirATransformacaoDepoisDeUpload()
    {
        CloudinaryUrlHelper.Optimize(Url, 480)
            .Should().Be("https://res.cloudinary.com/demo/image/upload/f_auto,q_auto,w_480/v1/foto.png");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Optimize_DeveDevolverVazio_QuandoNaoHaUrl(string? url)
    {
        CloudinaryUrlHelper.Optimize(url, 480).Should().BeEmpty();
    }

    [Fact]
    public void Optimize_DeveManterUrlQueNaoEhDoCloudinary()
    {
        CloudinaryUrlHelper.Optimize("https://exemplo.com/a.png", 480).Should().Be("https://exemplo.com/a.png");
    }

    [Fact]
    public void BuildSrcSet_DeveListarUmaVersaoPorLargura_NaOrdemInformada()
    {
        var srcset = CloudinaryUrlHelper.BuildSrcSet(Url, new[] { 480, 800, 1200 });

        srcset.Should().Be(
            "https://res.cloudinary.com/demo/image/upload/f_auto,q_auto,w_480/v1/foto.png 480w, " +
            "https://res.cloudinary.com/demo/image/upload/f_auto,q_auto,w_800/v1/foto.png 800w, " +
            "https://res.cloudinary.com/demo/image/upload/f_auto,q_auto,w_1200/v1/foto.png 1200w");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("https://exemplo.com/a.png")]
    public void BuildSrcSet_DeveDevolverNulo_QuandoNaoEhUrlDoCloudinary(string? url)
    {
        CloudinaryUrlHelper.BuildSrcSet(url, new[] { 480, 800 }).Should().BeNull();
    }
}