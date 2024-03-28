using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Recipes.Impl;

internal class RecipeService : IRecipeService
{
    private readonly Common.Config.DomainModel.Mealie appSettings;

    private readonly HttpClient httpClient;

    /// <summary>
    ///     Initializes an instance of the MealieService
    /// </summary>
    public RecipeService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        appSettings = appSettingsReader.GetAppSettings().Mealie;
        httpClient = httpClientFactory.BuildHttpClient();
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

        UpdateRecipeRequest? recipesResponse =
            await httpClient.GetFromJsonAsync<UpdateRecipeRequest>(url).ConfigureAwait(false);

        return recipesResponse;
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