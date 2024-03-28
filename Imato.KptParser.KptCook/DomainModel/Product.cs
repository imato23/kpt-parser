namespace Imato.KptParser.KptCook.DomainModel;

public class Product : KptModel
{
    public string? Title { get; set; }
    public string? Ingredient { get; set; }
    public string? Retailer { get; set; }
    public double? Price { get; set; }
    public double? NewQuantity { get; set; }
    public string? Measure { get; set; }
    public KptDate? CreationDate { get; set; }
    public KptDate? UpdateDate { get; set; }
}