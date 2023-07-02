namespace Imato.KptParser.KptCook.DomainModel;

public class Recipe : KptCookModel
{
    public required LocalizedString LocalizedTitle { get; set; }
    public required string RType { get; set; }
    public required LocalizedString AuthorComment { get; set; }
    public required string Country { get; set; }
    public required int PreparationTime { get; set; }
    public int? CookingTime { get; set; }
    public required RecipeNutrition RecipeNutrition { get; set; }
    public required string[] StepsDE { get; set; }
    public required IEnumerable<Author> Authors { get; set; }
    public required IEnumerable<IngredientInfo> Ingredients { get; set; }
    public required IEnumerable<Image> ImageList { get; set; }
}