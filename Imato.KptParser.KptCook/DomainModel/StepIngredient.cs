namespace Imato.KptParser.KptCook.DomainModel;

public class StepIngredient
{
    public Unit? Unit { get; set; }
    public string? IngredientId { get; set; }
    public required LocalizedString Title { get; set; }
}
