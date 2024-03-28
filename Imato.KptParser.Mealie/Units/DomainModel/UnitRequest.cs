namespace Imato.KptParser.Mealie.Units.DomainModel;

public class UnitRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Fraction { get; set; }
    public string? Abbreviation { get; set; }
    public bool UseAbbreviation { get; set; }
}