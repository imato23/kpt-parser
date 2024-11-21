namespace Imato.KptParser.KptCook.DomainModel;

public class Image
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public string? Type { get; set; }
}
