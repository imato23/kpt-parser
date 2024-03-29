using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Recipes;

public interface IRecipeService
{
    Task<RecipesResponse?> GetAllRecipesAsync();

    Task<UpdateRecipeRequest?> GetRecipeAsync(string slug);
    Task<string> AddRecipeAsync(RecipeRequest recipe);
    Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe);

    Task<bool> RecipeExistsAsync(string slug);

    string Slugify(string recipeTitle);
}