using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Foods.DomainModel;

public class Food
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? Extras { get; set; }
    public string? LabelId { get; set; }
    public string? Id { get; set; }
    public MultiPurposeLabelSummary? Label { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}