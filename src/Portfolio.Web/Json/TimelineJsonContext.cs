using System.Text.Json.Serialization;
using Portfolio.Web.DTOs.Timeline;

namespace Portfolio.Web.Json;

[JsonSerializable(typeof(List<TimelineEventDto>))]
[JsonSerializable(typeof(TimelineEventDto))]
[JsonSerializable(typeof(CreateTimelineEventDto))]
[JsonSerializable(typeof(UpdateTimelineEventDto))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public partial class TimelineJsonContext : JsonSerializerContext
{
}