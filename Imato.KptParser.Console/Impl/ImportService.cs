using System.Text.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.KptCook;
using Imato.KptParser.KptCook.DomainModel;
using Imato.KptParser.Mealie.Authorization;
using Imato.KptParser.Mealie.Categories;
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

internal class ImportService : IImportService
{
    private const string CachedIdsFileName = "cached-ids.json";
    private readonly AppSettings appSettings;
    private readonly IKptCookService kptCookService;
    private readonly ILogger<Worker> logger;
    private readonly IRecipeService recipeService;
    private readonly IFoodService foodService;
    private readonly IRecipeCategoryService recipeCategoryService;
    private readonly IUnitService unitService;
    private readonly IAuthorizationService authorizationService;

    public ImportService(
       ILogger<Worker> logger,
       IAppSettingsReader appSettingsReader,
       IKptCookService kptCookService,
       IRecipeService recipeService,
       IFoodService foodService,
       IRecipeCategoryService recipeCategoryService,
       IUnitService unitService,
       IAuthorizationService authorizationService)
    {
        this.logger = logger;
        appSettings = appSettingsReader.GetAppSettings();
        this.kptCookService = kptCookService;
        this.recipeService = recipeService;
        this.foodService = foodService;
        this.recipeCategoryService = recipeCategoryService;
        this.unitService = unitService;
        this.authorizationService = authorizationService;
    }

    public async Task StartImportAsync()
    {
        await authorizationService.LoginAsync().ConfigureAwait(false);
        
        IEnumerable<string> kptCookFavoriteIds = (await kptCookService.GetFavoriteIdsAsync().ConfigureAwait(false)).ToList();
        
        IEnumerable<string> kptCookIdsToImport = kptCookFavoriteIds;
        
        if (File.Exists(CachedIdsFileName))
        {   
            logger.LogInformation("Loading already imported KptCook favorite identifiers from JSON file");
            string jsonString1 = await File.ReadAllTextAsync(CachedIdsFileName).ConfigureAwait(false);
            IEnumerable<string>? alreadyImportedKptCookIds = JsonSerializer.Deserialize<IEnumerable<string>>(jsonString1);
            kptCookIdsToImport = kptCookFavoriteIds.Except(alreadyImportedKptCookIds ?? throw new InvalidOperationException()).ToList();

            if (!kptCookIdsToImport.Any())
            {
                logger.LogInformation("No new KptCook recipes to import");
                return;
            }
        }
        
        IEnumerable<Recipe> kptCookRecipes = await kptCookService.GetRecipesAsync(kptCookIdsToImport).ConfigureAwait(false);

        int currentRecipeCount = 1;

        IEnumerable<Recipe> cookRecipes = kptCookRecipes.ToList();
        
        foreach (Recipe kptCookRecipe in cookRecipes)
        {
            await CreateMealieRecipeAsync(kptCookRecipe, currentRecipeCount++, cookRecipes.Count()).ConfigureAwait(false);
        }
        
        logger.LogInformation("Saving already imported KptCook favorite identifiers to JSON file");
        string jsonString = JsonSerializer.Serialize(kptCookFavoriteIds);
        await File.WriteAllTextAsync(CachedIdsFileName, jsonString).ConfigureAwait(false);
    }

    private async Task CreateMealieRecipeAsync(Recipe kptCookRecipe, int currentCount, int maxCount)
    {
        bool recipeExists = await recipeService.RecipeWithNameExistsAsync(kptCookRecipe.LocalizedTitle.De).ConfigureAwait(false);

        if (recipeExists)
        {
            logger.LogDebug("Mealie recipe {RecipeName} already exists ({CurrentRecipeCount} of {MaxRecipeCount})",
                kptCookRecipe.LocalizedTitle.De,
                currentCount,
                maxCount);
            return;
        }

        logger.LogInformation(
                "Creating Mealie recipe for {RecipeName} ({CurrentRecipeCount} of {MaxRecipeCount})",
                kptCookRecipe.LocalizedTitle.De,
                currentCount,
                maxCount);

        string slug = await CreateRecipeAsync(kptCookRecipe).ConfigureAwait(false);
        IEnumerable<StepImage> stepImages = GetStepImages(kptCookRecipe.ImageList).ToList();

        await recipeService.UploadImagesForRecipeStepsAsync(slug, stepImages).ConfigureAwait(false);
        await UpdateRecipeAsync(kptCookRecipe, slug).ConfigureAwait(false);
    }

    private static IEnumerable<StepImage> GetStepImages(IEnumerable<Image> kptCookImages)
    {
        int index = 1;

        return (kptCookImages.Where(kptCookImage => kptCookImage.Type is "step" or null)
            .Select(kptCookImage =>
                new StepImage { FileName = BuildStepImageFileName(index++), ImageUrl = kptCookImage.Url })).ToList();
    }

    private static string BuildStepImageFileName(int index){
        return "step" + index.ToString("D2");
    }

    private async Task<string> CreateRecipeAsync(Recipe kptCookRecipe)
    {
        string title = kptCookRecipe.LocalizedTitle.De;
        string imageUrl = kptCookRecipe.ImageList.First(img => img.Type == "cover").Url;
        imageUrl = $"{imageUrl}?kptkey={appSettings.KptCook.ApiKey}";

        RecipeRequest mealieRecipe = new()
        {
            RecipeName = title,
            RecipeImageUrl = imageUrl
        };

        try
        {
            return await recipeService.AddRecipeAsync(mealieRecipe).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Couldn't create recipe '{Title}'", title);
            throw;
        }
    }

    private async Task UpdateRecipeAsync(Recipe kptCookRecipe, string slug)
    {
        UpdateRecipeRequest? updateRecipe = await recipeService.GetRecipeAsync(slug).ConfigureAwait(false);

        if (updateRecipe == null)
        {
            throw new InvalidOperationException($"Recipe for slug '{slug}' does not exist");
        }

        updateRecipe.DateUpdated = DateTime.Now;
        updateRecipe.Description = kptCookRecipe.AuthorComment.De;
        updateRecipe.PerformTime = kptCookRecipe.CookingTime?.ToString() ?? string.Empty;
        updateRecipe.CookTime = updateRecipe.PerformTime;
        updateRecipe.PrepTime = kptCookRecipe.PreparationTime.ToString();
        updateRecipe.TotalTime = (kptCookRecipe.CookingTime + kptCookRecipe.PreparationTime).ToString() ?? string.Empty;
        updateRecipe.RecipeYield = "1";

        if (!string.IsNullOrWhiteSpace(kptCookRecipe.Country))
        {
            updateRecipe.Notes.Add(new RecipeNote { Title = "Herkunftsland" , Text = kptCookRecipe.Country});
        }

        updateRecipe.RecipeCategory = await MapCategoryAsync(kptCookRecipe.Rtype).ConfigureAwait(false);
        updateRecipe.Nutrition = MapNutrition(kptCookRecipe.RecipeNutrition);
        updateRecipe.RecipeIngredient = await MapIngredientsAsync(kptCookRecipe.Ingredients).ConfigureAwait(false);
        updateRecipe.RecipeInstructions = MapInstructions(kptCookRecipe.Steps, updateRecipe.RecipeIngredient, updateRecipe.Id);

        try
        {
            await recipeService.UpdateRecipeAsync(slug, updateRecipe).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Couldn't update recipe for slug '{Slug}'", slug);
            throw;
        }
    }

    private async Task<IEnumerable<RecipeCategory>> MapCategoryAsync(string category)
    {
        RecipeCategory recipeCategory = await recipeCategoryService.GetOrAddCategoryAsync(category).ConfigureAwait(false);

        return new List<RecipeCategory> { recipeCategory };
    }

    private async Task<IEnumerable<Mealie.Recipes.DomainModel.RecipeIngredient>> MapIngredientsAsync(IEnumerable<RecipeIngredient> ingredients)
    {
        IList<Mealie.Recipes.DomainModel.RecipeIngredient> result = new List<Mealie.Recipes.DomainModel.RecipeIngredient>();

        foreach (RecipeIngredient kptCookIngredient in ingredients)
        {
            Unit? unit = null;

            if (kptCookIngredient.Measure != null)
            {
                unit = await unitService.GetOrAddUnitAsync(kptCookIngredient.Measure, kptCookIngredient.Measure).ConfigureAwait(false);
            }

            Food food = await foodService.GetOrAddFoodAsync(kptCookIngredient.Ingredient.LocalizedTitle.De).ConfigureAwait(false);

            Mealie.Recipes.DomainModel.RecipeIngredient mealieIngredient = new Mealie.Recipes.DomainModel.RecipeIngredient
            {
                Food = new IngredientFood
                {
                    Id = food.Id,
                    Name = kptCookIngredient.Ingredient.LocalizedTitle.De,
                    Description = string.Empty
                },
                Quantity = kptCookIngredient.Quantity,
                ReferenceId = food.Id
            };

            if (unit != null)
            {
                mealieIngredient.Unit = new IngredientUnit
                {
                    Id = unit.Id,
                    Abbreviation = string.Empty,
                    Name = string.Empty,
                    Description = string.Empty
                };
            }

            mealieIngredient.KptCookId = kptCookIngredient.Ingredient.OidObject.Oid;

            result.Add(mealieIngredient);
        }

        return result;
    }

    private string BuildStepImageUrl(string recipeId, int stepNumber)
    {
        string fileName = $"step{stepNumber:D2}.jpg";
        string url = @$"<img src=""/api/media/recipes/{recipeId}/assets/{fileName}"" height=""100%"" width=""100%""/>";
        return url;
    }

    private IEnumerable<RecipeStep> MapInstructions(
        List<Step> srcSteps,
        IEnumerable<Mealie.Recipes.DomainModel.RecipeIngredient> recipeIngredients,
        string recipeId)
    {
        IList<RecipeStep> result = new List<RecipeStep>();
        IEnumerable<Mealie.Recipes.DomainModel.RecipeIngredient> ingredients = recipeIngredients.ToList();

        int stepNumber = 1;

        foreach (Step step in srcSteps)
        {
            RecipeStep mealieStep = new RecipeStep
            {
                Id = Guid.NewGuid().ToString(),
                Title = string.Empty,
                Text = step.Title?.De + BuildStepImageUrl(recipeId, stepNumber++),
                IngredientReferences = new List<IngredientReference>()
            };

            if (step.Ingredients != null)
            {
                foreach(StepIngredient ingredient in step.Ingredients)
                {
                    Mealie.Recipes.DomainModel.RecipeIngredient? mealieIngredient =
                        ingredients.SingleOrDefault(x => x.KptCookId == ingredient.IngredientId);

                    if (mealieIngredient == null)
                    {
                        logger.LogWarning("Ingredient reference was not found for ingredient '{Ingredient}'", ingredient.Title.De);
                        continue;
                    }

                    mealieStep.IngredientReferences.Add(
                        new IngredientReference{ReferenceId = mealieIngredient.ReferenceId});
                }
            }

            result.Add(mealieStep);
        }

        return result;
    }

    private static Nutrition MapNutrition(RecipeNutrition nutrition)
    {
        const string notApplicable = "N/A";

        return new Nutrition
        {
            Calories = nutrition.Calories.ToString(),
            CarbohydrateContent = nutrition.Carbohydrate.ToString(),
            FatContent = nutrition.Fat.ToString(),
            FiberContent = notApplicable,
            ProteinContent = nutrition.Protein.ToString(),
            SodiumContent = notApplicable,
            SugarContent = notApplicable
        };
    }
}
