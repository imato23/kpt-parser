namespace Imato.KptParser.Mealie.Recipes.DomainModel;

public class IngredientUnit
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public object? Extras { get; set; }
    public bool Fraction { get; set; }
    public string? Abbreviation { get; set; }
    public bool UseAbbreviation { get; set; }
    public string? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}