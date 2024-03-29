using Imato.KptParser.Mealie.Foods.DomainModel;
using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Categories;

public interface IRecipeCategoryService
{
    Task<RecipeCategory> GetOrAddCategoryAsync(string name);
}