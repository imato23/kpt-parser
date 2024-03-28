namespace Imato.KptParser.KptCook.DomainModel;

public class Recipe : KptModel
{
    public LocalizedString? LocalizedTitle { get; set; }
    public string? Rtype { get; set; }
    public string? Gdocs { get; set; }
    public LocalizedString? AuthorComment { get; set; }
    public string? Uid { get; set; }
    public string? Country { get; set; }
    public int? PreparationTime { get; set; }
    public int? CookingTime { get; set; }
    public RecipeNutrition? RecipeNutrition { get; set; }
    public List<Step>? Steps { get; set; }
    public List<Author>? Authors { get; set; }
    public List<Retailer>? Retailers { get; set; }
    public List<RecipeIngredient>? Ingredients { get; set; }
    public List<Image>? ImageList { get; set; }
    public LocalizedDate? LocalizedPublishDate { get; set; }
    public LocalizedDates? PublishDates { get; set; }
    public string? TrackingMode { get; set; }
    public string? IsStandRecipe { get; set; }
    public PublishDuration? PublishDuration { get; set; }
    public string? IngredientTags { get; set; }
    public int? FavoriteCount { get; set; }
    public KptDate? CreationDate { get; set; }
    public KptDate? UpdateDate { get; set; }
}