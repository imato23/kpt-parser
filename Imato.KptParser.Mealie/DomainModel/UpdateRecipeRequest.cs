namespace Imato.KptParser.Mealie.DomainModel;

public class UpdateRecipeRequest
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string GroupId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }

    public string Image { get; set; }
    public string RecipeYield { get; set; }
    public string TotalTime { get; set; }
    public string PrepTime { get; set; }
    public string CookTime { get; set; }
    public string PerformTime { get; set; }

    public string Description { get; set; }
    public IEnumerable<RecipeCategory>? RecipeCategory { get; set; }
    public IEnumerable<RecipeTag>? Tags { get; set; }
    public IEnumerable<RecipeTool>? Tools { get; set; }
    public int? Rating { get; set; }
    public string OrgUrl { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? DateUpdated { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? LastMade { get; set; }
    public IEnumerable<RecipeIngredient>? RecipeIngredient { get; set; }
    public IEnumerable<RecipeStep>? RecipeInstructions { get; set; }
    public Nutrition? Nutrition { get; set; }
    public RecipeSettings? Settings { get; set; }
    public IEnumerable<RecipeAsset>? Assets { get; set; }

    public IEnumerable<RecipeNote>? Notes { get; set; }

    public object? Extras { get; set; }
    public bool IsOcrRecipe { get; set; }
    public IEnumerable<RecipeComment>? Comments { get; set; }
}