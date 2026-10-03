// Título: BlogStatusHelper.cs
// Descrição: Status efetivo de um post no admin (Scheduled vencido já está publicado no site)

namespace Portfolio.Web.Helpers;

/// <summary>
/// A API guarda Status = Scheduled mesmo depois que a data passa; a regra pública já trata
/// esse post como publicado. Este helper alinha o admin com o que o site mostra.
/// A hora atual vem por parâmetro para o helper ser testável com datas fixas.
/// </summary>
public static class BlogStatusHelper
{
    public const string Draft = "Draft";
    public const string Scheduled = "Scheduled";
    public const string Published = "Published";

    /// <summary>
    /// Devolve "Published" para um post "Scheduled" cuja data (UTC) já chegou.
    /// Nos demais casos devolve o status original. scheduledAtUtc e utcNow devem estar em UTC.
    /// </summary>
    public static string GetEffectiveStatus(string status, DateTime? scheduledAtUtc, DateTime utcNow)
    {
        if (status == Scheduled && scheduledAtUtc.HasValue && scheduledAtUtc.Value <= utcNow)
        {
            return Published;
        }

        return status;
    }

    /// <summary>
    /// True quando o post é Scheduled e já foi publicado automaticamente.
    /// </summary>
    public static bool IsOverdueScheduled(string status, DateTime? scheduledAtUtc, DateTime utcNow) =>
        status == Scheduled && GetEffectiveStatus(status, scheduledAtUtc, utcNow) == Published;
}
