using System.Text.Json.Serialization;

namespace Imato.KptParser.KptCook.DomainModel;

public class KptDate
{
    [JsonPropertyName("$date")] public long? Date { get; set; }
}