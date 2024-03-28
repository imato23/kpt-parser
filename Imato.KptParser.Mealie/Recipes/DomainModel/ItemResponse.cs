// ReSharper disable InconsistentNaming
namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class ItemResponse<TItem>
{
    public int Page { get; set; }
    public int Per_Page { get; set; }
    public int Total { get; set; }
    public int Total_Pages { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public IEnumerable<TItem>? Items { get; set; }
}