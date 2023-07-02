namespace Imato.KptParser.Mealie.DomainModel;

public class RecipeStep
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public IEnumerable<IngredientReference> IngredientReferences { get; set; }
}