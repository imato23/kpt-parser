using Imato.KptParser.KptCook.DomainModel;

namespace Imato.KptParser.KptCook;

public interface IKptCookService
{
    /// <summary>
    ///     Gets the identifiers of all KptCook favorites
    /// </summary>
    /// <returns>The favorite identifiers</returns>
    Task<IEnumerable<string>> GetFavoriteIdsAsync();

    /// <summary>
    ///     Gets the recipes with the specified identifiers
    /// </summary>
    /// <param name="recipeIds">The recipe identifiers</param>
    /// <returns>The recipes</returns>
    Task<IEnumerable<Recipe>?> GetRecipesAsync(IEnumerable<string> recipeIds);

    /// <summary>
    ///     Gets all favorite recipes
    /// </summary>
    /// <returns>The favorite recipes</returns>
    Task<IEnumerable<Recipe>?> GetFavoriteRecipesAsync();
}