namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class RecipeComment
{
    public string? RecipeId { get; set; }
    public string? Text { get; set; }
    public string? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
}