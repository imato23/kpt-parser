namespace Imato.KptParser.KptCook.DomainModel;

public class RecipeIngredientDetails : KptModel
{
    public RecipeIngredientDetails()
    {
    }

    public string? Typ { get; set; }
    public LocalizedString? LocalizedTitle { get; set; }
    public NumberTitle? NumberTitle { get; set; }
    public LocalizedString? UncountableTitle { get; set; }
    public string? Category { get; set; }
    public string? Key { get; set; }
    public string? Desc { get; set; }
    public Image? Image { get; set; }
    public List<Product>? Products { get; set; }
    public bool? IsSponsored { get; set; }
    public Measure? Measures { get; set; }
    public Synonym? Synonym { get; set; }
    public KptDate? CreationDate { get; set; }
    public KptDate? UpdateDate { get; set; }
}
