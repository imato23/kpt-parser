using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Console.DomainModel;
using Imato.KptParser.KptCook;
using Imato.KptParser.KptCook.DomainModel;
using Imato.KptParser.Mealie;
using Imato.KptParser.Mealie.Authorization;
using Imato.KptParser.Mealie.Foods;
using Imato.KptParser.Mealie.Recipes;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Imato.KptParser.Mealie.Units;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recipe = Imato.KptParser.KptCook.DomainModel.Recipe;

namespace Imato.KptParser.Console;

public class Worker : IHostedService
{
    private readonly AppSettings appSettings;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IKptCookService kptCookService;

    private readonly ILogger<Worker> logger;
    private readonly IRecipeService recipeService;
    private readonly IFoodService foodService;
    private readonly IUnitService unitService;
    private readonly IAuthorizationService authorizationService;

    // ReSharper disable once NotAccessedField.Local
    private readonly IOptions<CommandlineOptions> options;

    private int? exitCode;

    public Worker(
        ILogger<Worker> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<CommandlineOptions> options,
        IAppSettingsReader appSettingsReader,
        IKptCookService kptCookService,
        IRecipeService recipeService,
        IFoodService foodService,
        IUnitService unitService,
        IAuthorizationService authorizationService)
    {
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.options = options;
        appSettings = appSettingsReader.GetAppSettings();
        this.kptCookService = kptCookService;
        this.recipeService = recipeService;
        this.foodService = foodService;
        this.unitService = unitService;
        this.authorizationService = authorizationService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application");

        try
        {
            await authorizationService.LoginAsync().ConfigureAwait(false);

            await unitService.GetOrAddUnitAsync("Liter", "l");
            await foodService.GetOrAddFoodAsync("Schmand").ConfigureAwait(false);

            return;
            
            var favoriteIds = await kptCookService.GetFavoriteIdsAsync().ConfigureAwait(false);
            var kptCookRecipes = await kptCookService.GetRecipesAsync(favoriteIds);

            if (kptCookRecipes == null)
            {
                logger.LogWarning("No KptCook recipes have been found");
                return;
            }

            foreach (Recipe kptCookRecipe in kptCookRecipes)
                await CreateMealieRecipeAsync(kptCookRecipe).ConfigureAwait(false);

            exitCode = 0;
            await Task.CompletedTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("The application has been killed with CTRL+C");
            exitCode = -1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred");
            exitCode = 1;
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = exitCode.GetValueOrDefault(-1);
        logger.LogInformation("Shutting down the application with code {ExitCode}", Environment.ExitCode);
        return Task.CompletedTask;
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
        updateRecipe.RecipeInstructions = MapInstructions(kptCookRecipe.StepsDE, kptCookRecipe.ImageList);
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

    private IEnumerable<RecipeIngredient>? MapIngredients(IEnumerable<IngredientInfo> ingredients)
    {
        IList<RecipeIngredient> result = new List<RecipeIngredient>();

        foreach (IngredientInfo kptCookIngredient in ingredients)
        {
            RecipeIngredient mealieIngredient = new RecipeIngredient
            {
                // Food = new IngredientFood
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     Name = kptCookIngredient.Ingredient.LocalizedTitle.De,
                // },
                Quantity = kptCookIngredient.Quantity
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

    private IEnumerable<RecipeStep> MapInstructions(string[] steps, IEnumerable<Image> imageList)
    {
        IList<RecipeStep> result = new List<RecipeStep>();

        foreach (string step in steps)
            result.Add(new RecipeStep
            {
                Id = Guid.NewGuid().ToString(),
                Title = string.Empty,
                Text = step,
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