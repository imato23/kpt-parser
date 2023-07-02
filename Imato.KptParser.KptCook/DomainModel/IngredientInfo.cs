namespace Imato.KptParser.KptCook.DomainModel;

public class IngredientInfo
{
    public double Quantity { get; set; }
    public string? Measure { get; set; }
    public required Ingredient Ingredient { get; set; }
}