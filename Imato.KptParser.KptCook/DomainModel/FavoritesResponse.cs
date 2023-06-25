namespace Imato.KptCookImporter.KptCook.DomainModel;

public class FavoritesResponse
{
    public int? FavSpace { get; set; }
    public required IEnumerable<string> Favorites { get; set; }
}