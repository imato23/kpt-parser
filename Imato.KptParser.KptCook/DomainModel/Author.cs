namespace Imato.KptParser.KptCook.DomainModel;

public class Author : KptCookModel
{
    public required string Name { get; set; }
    public required string Link { get; set; }
    public required string Title { get; set; }
    public required LocalizedString LocalizedDesc { get; set; }
    public string? Sponsor { get; set; }
    public required Image AuthorImage { get; set; }

    // CreationDate
    // UpdateDate
}