using System.Text.Json.Serialization;

namespace Imato.KptParser.KptCook.DomainModel;

public abstract class KptCookModel
{
    [JsonPropertyName("_id")] public required OidObject OidObject { get; set; }
}