namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class RecipeStep
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public IList<IngredientReference>? IngredientReferences { get; set; }
}
