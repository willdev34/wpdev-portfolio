// Título: SeoUrlHelper.cs
// Descrição: Monta URLs absolutas (canonical, og:url) a partir do domínio público definitivo

namespace Portfolio.Web.Helpers;

/// <summary>
/// Crawlers de preview exigem URL absoluta. O domínio canônico é o www (o apex redireciona 308 para ele).
/// </summary>
public static class SeoUrlHelper
{
    public const string BaseUrl = "https://www.wpdevbr.com";

    /// <summary>
    /// Junta o domínio com o caminho da página. Aceita caminho vazio, nulo, com ou sem barra inicial.
    /// A raiz vira "https://www.wpdevbr.com/" e os demais caminhos não terminam em barra duplicada.
    /// </summary>
    public static string BuildFullUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == "/")
        {
            return $"{BaseUrl}/";
        }

        return path.StartsWith('/') ? $"{BaseUrl}{path}" : $"{BaseUrl}/{path}";
    }
}
