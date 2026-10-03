// Título: BrasiliaTimeHelper.cs
// Descrição: Conversão entre UTC e horário de Brasília com offset fixo de -3 horas

using System.Globalization;

namespace Portfolio.Web.Helpers;

/// <summary>
/// Converte datas entre UTC (formato gravado na API) e horário de Brasília (formato exibido no admin).
/// O Brasil não tem horário de verão desde 2019, então o offset é fixo em -3 horas.
/// Não usa TimeZoneInfo.FindSystemTimeZoneById porque os dados de fuso podem não existir no WASM em Release.
/// </summary>
public static class BrasiliaTimeHelper
{
    public static readonly TimeSpan Offset = TimeSpan.FromHours(-3);

    /// <summary>
    /// Converte um horário digitado em Brasília (ex: input datetime-local) para UTC com Kind Utc.
    /// </summary>
    public static DateTime ToUtc(DateTime brasiliaLocal)
    {
        var unspecified = DateTime.SpecifyKind(brasiliaLocal, DateTimeKind.Unspecified);
        return new DateTimeOffset(unspecified, Offset).UtcDateTime;
    }

    /// <summary>
    /// Converte um horário UTC vindo da API para Brasília (Kind Unspecified, pronto para o input).
    /// </summary>
    public static DateTime FromUtc(DateTime utc)
    {
        var asUtc = utc.Kind switch
        {
            DateTimeKind.Local => utc.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            _ => utc
        };
        return new DateTimeOffset(asUtc).ToOffset(Offset).DateTime;
    }

    /// <summary>
    /// Agora em horário de Brasília.
    /// </summary>
    public static DateTime Now => FromUtc(DateTime.UtcNow);

    /// <summary>
    /// Formato do valor de um input datetime-local (sem segundos), para value e min.
    /// </summary>
    public const string InputFormat = "yyyy-MM-ddTHH:mm";

    // O navegador omite os segundos quando são zero, mas pode mandar segundos e milissegundos
    private static readonly string[] InputFormats =
    {
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff"
    };

    /// <summary>
    /// Formata um horário de Brasília para o value ou o min de um input datetime-local.
    /// </summary>
    public static string ToInputValue(DateTime brasilia) =>
        brasilia.ToString(InputFormat, CultureInfo.InvariantCulture);

    /// <summary>
    /// Converte o texto de um input datetime-local (horário de Brasília) para DateTime com Kind Unspecified.
    /// Retorna false para texto vazio, nulo ou fora dos formatos aceitos.
    /// </summary>
    public static bool TryParseInput(string? valor, out DateTime brasilia)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            brasilia = default;
            return false;
        }

        return DateTime.TryParseExact(
            valor.Trim(),
            InputFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out brasilia);
    }
}
