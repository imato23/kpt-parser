using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Console.DomainModel;
using Imato.KptParser.KptCook;
using Imato.KptParser.Mealie;
using Imato.KptParser.Mealie.DomainModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recipe = Imato.KptParser.KptCook.DomainModel.Recipe;

namespace Imato.KptParser.Console;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    private readonly ILogger<Worker> logger;

    // ReSharper disable once NotAccessedField.Local
    private readonly IOptions<CommandlineOptions> options;
    private readonly IKptCookService kptCookService;
    private readonly IMealieService mealieService;

    private int? exitCode;
    private readonly AppSettings appSettings;

    public Worker(
        ILogger<Worker> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<CommandlineOptions> options,
        IAppSettingsReader appSettingsReader,
        IKptCookService kptCookService,
        IMealieService mealieService)
    {
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.options = options;
        this.appSettings = appSettingsReader.GetAppSettings();
        this.kptCookService = kptCookService;
        this.mealieService = mealieService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application");

        try
        {
            IEnumerable<string> favoriteIds = await kptCookService.GetFavoriteIdsAsync().ConfigureAwait(false);
            IEnumerable<Recipe>? kptCookRecipes = await kptCookService.GetRecipesAsync(favoriteIds);

            if (kptCookRecipes == null)
            {
                logger.LogWarning("No KptCook recipes have been found");
                return;
            }
            
            foreach (Recipe kptCookRecipe in kptCookRecipes)
            {
                string imageUrl = kptCookRecipe.ImageList.Single(img => img.Type == "cover").Url;
                imageUrl = $"{imageUrl}?kptkey={appSettings.KptCook.ApiKey}";
                
                RecipeRequest mealieRecipe = new RecipeRequest
                {
                    RecipeName = kptCookRecipe.LocalizedTitle.De,
                    RecipeImageUrl = imageUrl
                };
            
                await mealieService.AddRecipe(mealieRecipe).ConfigureAwait(false);
            }

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
}