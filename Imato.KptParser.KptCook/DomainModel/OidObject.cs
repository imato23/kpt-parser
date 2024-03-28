using System.Text.Json.Serialization;

namespace Imato.KptParser.KptCook.DomainModel;

public class OidObject
{
    [JsonPropertyName("$oid")] public string Oid { get; set; } = null!;
}