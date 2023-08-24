namespace Imato.KptParser.Mealie.Units.DomainModel;

public class Unit
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> Extras { get; set; }
    public bool Fraction { get; set; }
    public string Abbreviation { get; set; }
    public bool UseAbbreviation { get; set; }
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}