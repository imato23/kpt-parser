namespace Imato.KptParser.KptCook.DomainModel;

public class Ingredient : KptCookModel
{
    public required string Typ { get; set; }
    public required LocalizedString LocalizedTitle { get; set; }
    public required NumberTitle NumberTitle { get; set; }
    public required LocalizedString UncountableTitle { get; set; }
    public required string Category { get; set; }
    public required string Key { get; set; }
    public required string Desc { get; set; }

    public Image? Image { get; set; }
    // CreationDate
    // UpdateDate
}