using Imato.KptCookImporter.KptCook.DomainModel;

namespace Imato.KptCookImporter.KptCook;

public interface IKptCookService
{
    Task<IEnumerable<string>> GetFavoriteIdsAsync();
    Task<IEnumerable<Recipe>?> GetRecipesAsync(IEnumerable<string> recipeIds);
}