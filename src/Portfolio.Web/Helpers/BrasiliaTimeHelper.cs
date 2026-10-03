// Título: BrasiliaTimeHelper.cs
// Descrição: Conversão entre UTC e horário de Brasília com offset fixo de -3 horas

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
}
