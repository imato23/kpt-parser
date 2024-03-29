using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Microsoft.Extensions.Logging;

namespace Imato.KptParser.Mealie.Recipes.Impl;

internal class RecipeService : IRecipeService
{
    private readonly Common.Config.DomainModel.Mealie appSettings;

    private readonly HttpClient httpClient;
    private readonly ILogger<RecipeService> logger;

    /// <summary>
    ///     Initializes an instance of the MealieService
    /// </summary>
    public RecipeService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader, ILogger<RecipeService> logger)
    {
        appSettings = appSettingsReader.GetAppSettings().Mealie;
        httpClient = httpClientFactory.BuildHttpClient();
        this.logger = logger;
    }

    public async Task<RecipesResponse?> GetAllRecipesAsync()
    {
        var url = $"{appSettings.ApiUrl}/recipes";
        RecipesResponse? recipesResponse =
            await httpClient.GetFromJsonAsync<RecipesResponse>(url).ConfigureAwait(false);
        return recipesResponse;
    }

    public async Task<UpdateRecipeRequest?> GetRecipeAsync(string slug)
    {
        var url = $"{appSettings.ApiUrl}/recipes/{slug}";

        string json = await httpClient.GetStringAsync(url).ConfigureAwait(false);

        UpdateRecipeRequest? recipesResponse =
            await httpClient.GetFromJsonAsync<UpdateRecipeRequest>(url).ConfigureAwait(false);

        return recipesResponse;
    }

    public async Task<bool> RecipeExistsAsync(string slug)
    {
        try
        {
            var recipe = await GetRecipeAsync(slug).ConfigureAwait(false);

            return recipe != null;
        }
        catch (Exception e)
        {
            // todo: Handle specific exception
            return false;
        }
    }

    public async Task<string> AddRecipeAsync(RecipeRequest recipe)
    {
        string? slug = await CreateRecipeAsync(recipe.RecipeName).ConfigureAwait(false);

        if (slug == null) throw new InvalidOperationException("Couldn't create recipe");

        await UpdateRecipeImageAsync(slug, recipe.RecipeImageUrl).ConfigureAwait(false);

        return slug;
    }

    public async Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe)
    {
        var url = $"{appSettings.ApiUrl}/recipes/{slug}";

        HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, recipe).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            logger.LogCritical("Recipe for slug {Slug} couldn't be updated. Error Details: {ErrorDetails}", slug, content);
        }

        response.EnsureSuccessStatusCode();
    }

    private async Task<string?> CreateRecipeAsync(string name)
    {
        var url = $"{appSettings.ApiUrl}/recipes";

        CreateRecipeRequest body = new() { Name = name };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
        var slug = await response.Content.ReadFromJsonAsync<string>().ConfigureAwait(false);
        return slug;
    }

    private async Task UpdateRecipeImageAsync(string slug, string imageUrl)
    {
        var url = $"{appSettings.ApiUrl}/recipes/{slug}/image";

        ImageRequest body = new ImageRequest { Url = imageUrl };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}