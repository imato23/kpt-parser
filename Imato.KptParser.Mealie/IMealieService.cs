using Imato.KptParser.Mealie.DomainModel;

namespace Imato.KptParser.Mealie;

public interface IMealieService
{
    Task LoginAsync();
    Task<RecipesResponse?> GetAllRecipesAsync();

    Task<UpdateRecipeRequest?> GetRecipeAsync(string slug);
    Task<string> AddRecipeAsync(RecipeRequest recipe);
    Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe);
}