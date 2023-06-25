using System.Net.Http.Json;
using Imato.KptCookImporter.KptCook.DomainModel;

namespace Imato.KptCookImporter.KptCook.Impl;

public class KptCookService : IKptCookService
{
    private const string ApiUrl = "https://mobile.kptncook.com";
    private const string ApiKey = "6q7QNKy-oIgk-IMuWisJ-jfN7s6";
    private const string AccessToken = "70bb210f-2b3e-486c-9cad-28a0f60d93c5"; // Produktiv Konto
    private const string AccessTokenBak = "a4b31348-824b-49a9-afec-8717e47076b1"; // Test Konto

    private readonly HttpClient httpClient;

    public KptCookService()
    {
        httpClient = new HttpClient();
        AddHeaders();
    }

    public async Task<IEnumerable<string>> GetFavoriteIdsAsync()
    {
        var url = $"{ApiUrl}/favorites?kptnkey={ApiKey}";

        var response = await httpClient.GetFromJsonAsync<FavoritesResponse>(url).ConfigureAwait(false);

        if (response == null)
            throw new InvalidOperationException("Couldn't receive favorite identifiers from Kpt Cook Api");

        return response.Favorites;
    }

    public async Task<IEnumerable<Recipe>?> GetRecipesAsync(IEnumerable<string> recipeIds)
    {
        var url = $"{ApiUrl}/recipes/search?kptnkey={ApiKey}";
        var payload = recipeIds.Select(recipeId => new IdRequest { Identifier = recipeId });
        var response = await httpClient.PostAsJsonAsync(url, payload).ConfigureAwait(false);
        var recipes = await response.Content.ReadFromJsonAsync<IEnumerable<Recipe>>();

        return recipes;
    }

    private void AddHeaders()
    {
        //httpClient.DefaultRequestHeaders.Add("content-type", "application/json");
        // httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.kptncook.mobile-v8+json");
        // httpClient.DefaultRequestHeaders.Add("User-Agent", "Platform/Android/12.0.1 App/7.10.1");
        httpClient.DefaultRequestHeaders.Add("hasIngredients", "yes");
        httpClient.DefaultRequestHeaders.Add("Token", AccessToken);
    }
}