namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class RecipeIngredient
{
    public string? Title { get; set; }
    public string? Note { get; set; }
    public IngredientUnit? Unit { get; set; }
    public IngredientFood? Food { get; set; }
    public bool DisableAmount { get; set; }
    public double Quantity { get; set; }
    public string? OriginalText { get; set; }
    public string? ReferenceId { get; set; }
}