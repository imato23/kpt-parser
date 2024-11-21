using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.KptCook.DomainModel;
using Microsoft.Extensions.Logging;

namespace Imato.KptParser.KptCook.Impl;

internal class KptCookService : IKptCookService
{
    private readonly Common.Config.DomainModel.KptCook appSettings;
    private readonly HttpClient httpClient;
    private readonly ILogger<KptCookService> logger;

    /// <summary>
    ///     Initializes an instance of the KptCookService
    /// </summary>
    public KptCookService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader, ILogger<KptCookService> logger)
    {
        appSettings = appSettingsReader.GetAppSettings().KptCook;
        httpClient = httpClientFactory.BuildHttpClient();
        AddHeaders();
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetFavoriteIdsAsync()
    {
        logger.LogInformation("Getting identifiers of all favorites from KptCook API");

        // ReSharper disable once StringLiteralTypo
        string url = $"{appSettings.ApiUrl}/favorites?kptnkey={appSettings.ApiKey}";

        FavoritesResponse? response = await httpClient.GetFromJsonAsync<FavoritesResponse>(url).ConfigureAwait(false);

        if (response == null)
        {
            throw new InvalidOperationException("Couldn't receive favorite identifiers from Kpt Cook Api");
        }

        return response.Favorites;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Recipe>> GetRecipesAsync(IEnumerable<string> recipeIds)
    {
        logger.LogInformation("Getting recipes for favorites from KptCook API");

        string url = $"{appSettings.ApiUrl}/recipes/search?kptnkey={appSettings.ApiKey}";
        IEnumerable<IdObject> idObjects = recipeIds.Select(recipeId => new IdObject { Identifier = recipeId });
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, idObjects).ConfigureAwait(false);
        IEnumerable<Recipe>? recipes = await response.Content.ReadFromJsonAsync<IEnumerable<Recipe>>().ConfigureAwait(false);

        if (recipes == null)
        {
            throw new InvalidOperationException("Recipes list must not be null");
        }

        IEnumerable<Recipe> recipesAsync = recipes.ToList();
        
        logger.LogInformation("Found {RecipesCount} KptCook recipes", recipesAsync.Count());

        return recipesAsync;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Recipe>> GetFavoriteRecipesAsync()
    {
        IEnumerable<string> favoriteIds = await GetFavoriteIdsAsync().ConfigureAwait(false);
        IEnumerable<Recipe> recipes = await GetRecipesAsync(favoriteIds).ConfigureAwait(false);

        if (recipes == null)
        {
            throw new InvalidOperationException("Recipes list must not be null");
        }

        return recipes;
    }

    /// <summary>
    /// Adds the required headers to the request
    /// </summary>
    private void AddHeaders()
    {
        //httpClient.DefaultRequestHeaders.Add("content-type", "application/json");
        // httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.kptncook.mobile-v8+json");
        // httpClient.DefaultRequestHeaders.Add("User-Agent", "Platform/Android/12.0.1 App/7.10.1");
        httpClient.DefaultRequestHeaders.Add("hasIngredients", "yes");
        httpClient.DefaultRequestHeaders.Add("Token", appSettings.AccessToken);
    }
}
