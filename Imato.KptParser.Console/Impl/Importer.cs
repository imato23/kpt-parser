using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.KptCook;
using Imato.KptParser.KptCook.DomainModel;
using Imato.KptParser.Mealie.Authorization;
using Imato.KptParser.Mealie.Foods;
using Imato.KptParser.Mealie.Foods.DomainModel;
using Imato.KptParser.Mealie.Recipes;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Imato.KptParser.Mealie.Units;
using Microsoft.Extensions.Logging;
using Recipe = Imato.KptParser.KptCook.DomainModel.Recipe;
using RecipeIngredient = Imato.KptParser.KptCook.DomainModel.RecipeIngredient;
using Unit = Imato.KptParser.Mealie.Units.DomainModel.Unit;

namespace Imato.KptParser.Console.Impl;

internal class Importer : IImporter
{
    private readonly AppSettings appSettings;
    private readonly IKptCookService kptCookService;
    private readonly ILogger<Worker> logger;
    private readonly IRecipeService recipeService;
    private readonly IFoodService foodService;
    private readonly IUnitService unitService;
    private readonly IAuthorizationService authorizationService;

    public Importer(
       ILogger<Worker> logger,
       IAppSettingsReader appSettingsReader,
       IKptCookService kptCookService,
       IRecipeService recipeService,
       IFoodService foodService,
       IUnitService unitService,
       IAuthorizationService authorizationService)
    {
        this.logger = logger;
        appSettings = appSettingsReader.GetAppSettings();
        this.kptCookService = kptCookService;
        this.recipeService = recipeService;
        this.foodService = foodService;
        this.unitService = unitService;
        this.authorizationService = authorizationService;
    }

    public async Task StartImportAsync()
    {
        await authorizationService.LoginAsync().ConfigureAwait(false);

        Unit unit = await unitService.GetOrAddUnitAsync("Liter", "l").ConfigureAwait(false);
        Food food = await foodService.GetOrAddFoodAsync("Schmand").ConfigureAwait(false);

        IEnumerable<string> favoriteIds = await kptCookService.GetFavoriteIdsAsync().ConfigureAwait(false);
        IEnumerable<Recipe>? kptCookRecipes = await kptCookService.GetRecipesAsync(favoriteIds).ConfigureAwait(false);

        if (kptCookRecipes == null)
        {
            logger.LogWarning("No KptCook recipes have been found");
            return;
        }

        foreach (Recipe kptCookRecipe in kptCookRecipes)
            await CreateMealieRecipeAsync(kptCookRecipe).ConfigureAwait(false);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task CreateMealieRecipeAsync(Recipe kptCookRecipe)
    {
        string slug = "low-carb-paella-mit-garnelen";

        string imageUrl = kptCookRecipe.ImageList.Single(img => img.Type == "cover").Url;
        imageUrl = $"{imageUrl}?kptkey={appSettings.KptCook.ApiKey}";

        RecipeRequest mealieRecipe = new()
        {
            RecipeName = kptCookRecipe.LocalizedTitle.De,
            RecipeImageUrl = imageUrl
        };

        //string slug1 = await mealieService.AddRecipeAsync(mealieRecipe).ConfigureAwait(false);

        UpdateRecipeRequest? updateRecipe = await recipeService.GetRecipeAsync(slug).ConfigureAwait(false);

        if (updateRecipe == null)
        {
            throw new InvalidOperationException($"Recipe for slug '{slug}' does not exist");
        }

        updateRecipe.Description = kptCookRecipe.AuthorComment.De;
        updateRecipe.CookTime = kptCookRecipe.CookingTime?.ToString() ?? string.Empty;
        updateRecipe.PerformTime = kptCookRecipe.PreparationTime.ToString();
        updateRecipe.TotalTime = (kptCookRecipe.CookingTime + kptCookRecipe.PreparationTime).ToString() ?? string.Empty;
        updateRecipe.RecipeInstructions = MapInstructions(kptCookRecipe.Steps, kptCookRecipe.ImageList);
        updateRecipe.Nutrition = MapNutrition(kptCookRecipe.RecipeNutrition);
        updateRecipe.RecipeIngredient = MapIngredients(kptCookRecipe.Ingredients);

        // Todo: Add step images to recipe steps

        // Todo: Add the following data to mealie recipe
        //kptCookRecipe.Authors
        //kptCookRecipe.ImageList
        //kptCookRecipe.Country
        //kptCookRecipe.Ingredients

        try
        {
            await recipeService.UpdateRecipeAsync(slug, updateRecipe).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Couldn't update recipe for slug '{Slug}'", slug);
        }
    }

    private IEnumerable<Mealie.Recipes.DomainModel.RecipeIngredient>? MapIngredients(IEnumerable<RecipeIngredient>? ingredients)
    {
        IList<Mealie.Recipes.DomainModel.RecipeIngredient> result = new List<Mealie.Recipes.DomainModel.RecipeIngredient>();

        foreach (RecipeIngredient kptCookIngredient in ingredients)
        {
            Mealie.Recipes.DomainModel.RecipeIngredient mealieIngredient = new Mealie.Recipes.DomainModel.RecipeIngredient
            {
                // Food = new IngredientFood
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     Name = kptCookIngredient.Ingredient.LocalizedTitle.De,
                // },
                Quantity = (double)kptCookIngredient.Quantity!
            };

            // if (kptCookIngredient.Measure != null)
            // {
            //     mealieIngredient.Unit = new IngredientUnit
            //     {
            //         Id = Guid.NewGuid().ToString(),
            //         Name = kptCookIngredient.Measure,
            //         Abbreviation = kptCookIngredient.Measure
            //     };
            // }

            result.Add(mealieIngredient);
        }

        return result;
    }

    private IEnumerable<RecipeStep> MapInstructions(List<Step>? steps, IEnumerable<Image> imageList)
    {
        IList<RecipeStep> result = new List<RecipeStep>();

        foreach (Step step in steps)
            result.Add(new RecipeStep
            {
                Id = Guid.NewGuid().ToString(),
                Title = string.Empty,
                Text = step.Title?.De,
                IngredientReferences = new List<IngredientReference>()
            });

        return result;
    }

    private static Nutrition MapNutrition(RecipeNutrition nutrition)
    {
        return new Nutrition
        {
            Calories = nutrition.Calories.ToString(),
            CarbohydrateContent = nutrition.Carbohydrate.ToString(),
            FatContent = nutrition.Fat.ToString(),
            FiberContent = string.Empty,
            ProteinContent = nutrition.Protein.ToString(),
            SodiumContent = string.Empty
        };
    }
}
