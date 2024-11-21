using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Recipes;

public interface IRecipeService
{
    Task<RecipesResponse> GetAllRecipesAsync();
    Task<UpdateRecipeRequest?> GetRecipeAsync(string slug);
    Task<string> AddRecipeAsync(RecipeRequest recipe);
    Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe);
    Task<bool> RecipeWithSlugExistsAsync(string slug);
    Task<bool> RecipeWithNameExistsAsync(string name);
    
    Task UploadImagesForRecipeStepsAsync(string slug, IEnumerable<StepImage> images);
}
