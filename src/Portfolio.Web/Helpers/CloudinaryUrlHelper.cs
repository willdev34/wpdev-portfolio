// Título: CloudinaryUrlHelper.cs
// Descrição: Helper estático para aplicar transformações de otimização nas URLs do Cloudinary

namespace Portfolio.Web.Helpers;

/// <summary>
/// Aplica transformações de otimização (formato, qualidade, largura) em URLs do Cloudinary
/// sem precisar reprocessar a imagem no backend. A transformação é inserida na própria URL
/// logo após o segmento "/upload/".
/// </summary>
public static class CloudinaryUrlHelper
{
    /// <summary>
    /// Retorna a URL do Cloudinary com transformações de otimização aplicadas.
    /// Se a URL não for do Cloudinary (ex: link externo ou vazio), retorna a URL original sem alteração.
    /// </summary>
    /// <param name="url">URL original da imagem</param>
    /// <param name="width">Largura desejada em pixels (ex: 400 para thumbnail, 1200 para hero)</param>
    /// <param name="quality">Qualidade. Default "auto" deixa o Cloudinary decidir o melhor balanço.</param>
    /// <param name="format">Formato. Default "auto" entrega WebP/AVIF se o navegador suportar.</param>
    /// <returns>URL transformada, ou a URL original se não for possível transformar</returns>
    public static string Optimize(string? url, int width, string quality = "auto", string format = "auto")
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        const string uploadMarker = "/upload/";
        var markerIndex = url.IndexOf(uploadMarker, StringComparison.Ordinal);

        // Não é uma URL do Cloudinary no formato esperado, retorna como está
        if (markerIndex < 0)
        {
            return url;
        }

        var insertPosition = markerIndex + uploadMarker.Length;
        var transformation = $"f_{format},q_{quality},w_{width}/";

        return url.Insert(insertPosition, transformation);
    }

    /// <summary>
    /// Largura recomendada para thumbnails em grids de cards (Projects, Blog)
    /// </summary>
    public const int ThumbnailWidth = 480;

    /// <summary>
    /// Largura recomendada para imagens de destaque/hero (ProjectDetail, BlogPostDetail, Home featured)
    /// </summary>
    public const int HeroWidth = 1200;

    /// <summary>
    /// Largura recomendada para ícones pequenos (Timeline)
    /// </summary>
    public const int IconWidth = 80;
}