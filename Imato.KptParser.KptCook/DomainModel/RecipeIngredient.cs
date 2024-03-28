namespace Imato.KptParser.KptCook.DomainModel;

public class RecipeIngredient
{
    public double? Quantity { get; set; }
    public string? Measure { get; set; }
    public double? QuantityUs { get; set; }
    public string? MeasureUs { get; set; }
    public double? QuantityUsProd { get; set; }
    public string? MeasureUsProd { get; set; }
    public RecipeIngredientDetails? Ingredient { get; set; }
}