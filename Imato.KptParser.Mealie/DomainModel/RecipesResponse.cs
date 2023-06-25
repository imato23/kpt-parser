using System.Text.Json.Serialization;

namespace Imato.KptParser.Mealie.DomainModel;

public class RecipesResponse
{
    public int Page { get; set; }

    [JsonPropertyName("per_page")] public int ItemsPerPage { get; set; }

    public int Total { get; set; }

    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }

    public IEnumerable<Recipe> Items { get; set; }

    public string Next { get; set; }

    public string Previous { get; set; }
}