using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.KptCook.DomainModel;

namespace Imato.KptParser.KptCook.Impl;

internal class KptCookService : IKptCookService
{
    private readonly Common.Config.DomainModel.KptCook appSettings;
    private readonly HttpClient httpClient;

    /// <summary>
    ///     Initializes an instance of the KptCookService
    /// </summary>
    public KptCookService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        appSettings = appSettingsReader.GetAppSettings().KptCook;
        httpClient = httpClientFactory.BuildHttpClient();
        AddHeaders();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetFavoriteIdsAsync()
    {
        var url = $"{appSettings.ApiUrl}/favorites?kptnkey={appSettings.ApiKey}";

        FavoritesResponse? response = await httpClient.GetFromJsonAsync<FavoritesResponse>(url).ConfigureAwait(false);

        if (response == null)
            throw new InvalidOperationException("Couldn't receive favorite identifiers from Kpt Cook Api");

        return response.Favorites;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Recipe>?> GetRecipesAsync(IEnumerable<string> recipeIds)
    {
        var url = $"{appSettings.ApiUrl}/recipes/search?kptnkey={appSettings.ApiKey}";
        var idObjects = recipeIds.Select(recipeId => new IdObject { Identifier = recipeId });
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, idObjects).ConfigureAwait(false);
        var recipes = await response.Content.ReadFromJsonAsync<IEnumerable<Recipe>>();

        return recipes;
    }

    public async Task<IEnumerable<Recipe>?> GetFavoriteRecipesAsync()
    {
        var favoriteIds = await GetFavoriteIdsAsync().ConfigureAwait(false);
        var recipes = await GetRecipesAsync(favoriteIds).ConfigureAwait(false);

        return recipes;
    }

    private void AddHeaders()
    {
        //httpClient.DefaultRequestHeaders.Add("content-type", "application/json");
        // httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.kptncook.mobile-v8+json");
        // httpClient.DefaultRequestHeaders.Add("User-Agent", "Platform/Android/12.0.1 App/7.10.1");
        httpClient.DefaultRequestHeaders.Add("hasIngredients", "yes");
        httpClient.DefaultRequestHeaders.Add("Token", appSettings.AccessToken);
    }
}