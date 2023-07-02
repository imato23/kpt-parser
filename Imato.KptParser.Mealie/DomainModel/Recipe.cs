namespace Imato.KptParser.Mealie.DomainModel;

public class Recipe
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string GroupId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Image { get; set; }
    public required string RecipeYield { get; set; }
    public required string TotalTime { get; set; }
    public required string PrepTime { get; set; }
    public required string CookTime { get; set; }
    public required string PerformTime { get; set; }
    public required string Description { get; set; }
    public IEnumerable<RecipeCategory>? RecipeCategories { get; set; }
    public IEnumerable<RecipeTag>? Tags { get; set; }
    public IEnumerable<string>? Tools { get; set; }
    public int? Rating { get; set; }
    public required string OrgUrl { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? DateUpdated { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? LastMade { get; set; }
}