namespace Imato.KptParser.KptCook.DomainModel;

public class Author : KptModel
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? Title { get; set; }
    public LocalizedString? LocalizedDesc { get; set; }
    public string? Instagram { get; set; }
    public string? Sponsor { get; set; }
    public Image? AuthorImage { get; set; }
    public KptDate? CreationDate { get; set; }
    public KptDate? UpdateDate { get; set; }
}