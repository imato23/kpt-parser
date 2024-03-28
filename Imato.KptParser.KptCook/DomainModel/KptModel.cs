using System.Text.Json.Serialization;

namespace Imato.KptParser.KptCook.DomainModel;

public abstract class KptModel
{
    [JsonPropertyName("_id")] public required OidObject OidObject { get; set; }
}