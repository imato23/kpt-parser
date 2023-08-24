using System.Text.Json.Serialization;

namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class AccessTokenInfo
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = null!;
}