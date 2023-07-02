using Imato.KptParser.Mealie.DomainModel;

namespace Imato.KptParser.Mealie;

public interface IMealieService
{
    Task LoginAsync();
    Task<RecipesResponse?> GetAllRecipesAsync();
    Task AddRecipe(RecipeRequest recipe);
}