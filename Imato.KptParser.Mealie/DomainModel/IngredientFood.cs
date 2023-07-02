namespace Imato.KptParser.Mealie.DomainModel;

public class IngredientFood
{
    public string Name { get; set; }
    public string Description { get; set; }
    public object Extras { get; set; }
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string LabelId { get; set; }
    public MultiPurposeLabelSummary Label { get; set; }
}