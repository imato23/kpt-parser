using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.DomainModel;

namespace Imato.KptParser.Mealie.Impl;

internal class MealieService : IMealieService
{
    private readonly Common.Config.DomainModel.Mealie appSettings;

    private readonly HttpClient httpClient;

    private bool isLoggedIn;

    /// <summary>
    ///     Initializes an instance of the MealieService
    /// </summary>
    public MealieService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        appSettings = appSettingsReader.GetAppSettings().Mealie;
        httpClient = httpClientFactory.BuildHttpClient();
    }

    public async Task LoginAsync()
    {
        if (!isLoggedIn)
        {
            string accessToken = await FetchAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");
        }

        isLoggedIn = true;
    }

    public async Task<RecipesResponse?> GetAllRecipesAsync()
    {
        await LoginAsync().ConfigureAwait(false);
        var url = $"{appSettings.ApiUrl}/recipes";
        RecipesResponse? recipesResponse =
            await httpClient.GetFromJsonAsync<RecipesResponse>(url).ConfigureAwait(false);
        return recipesResponse;
    }

    public async Task<UpdateRecipeRequest?> GetRecipeAsync(string slug)
    {
        await LoginAsync().ConfigureAwait(false);

        var url = $"{appSettings.ApiUrl}/recipes/{slug}";

        UpdateRecipeRequest? recipesResponse =
            await httpClient.GetFromJsonAsync<UpdateRecipeRequest>(url).ConfigureAwait(false);

        return recipesResponse;
    }

    public async Task<string> AddRecipeAsync(RecipeRequest recipe)
    {
        string? slug = await CreateRecipeAsync(recipe.RecipeName).ConfigureAwait(false);

        if (slug == null) throw new InvalidOperationException("Couldn't create recipe");

        await UpdateRecipeImage(slug, recipe.RecipeImageUrl);

        return slug;
    }

    public async Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe)
    {
        await LoginAsync().ConfigureAwait(false);

        var url = $"{appSettings.ApiUrl}/recipes/{slug}";

        HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, recipe).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    private async Task<string> FetchAccessTokenAsync()
    {
        var url = $"{appSettings.ApiUrl}/auth/token";

        IEnumerable<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>
        {
            new("username", appSettings.Username),
            new("password", appSettings.Password)
        };

        using (HttpContent formContent = new FormUrlEncodedContent(formData))
        {
            using (HttpResponseMessage response = await httpClient.PostAsync(url, formContent).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                AccessTokenInfo? accessTokenInfo =
                    await response.Content.ReadFromJsonAsync<AccessTokenInfo>().ConfigureAwait(false);
                return accessTokenInfo!.AccessToken;
            }
        }
    }

    private async Task<string?> CreateRecipeAsync(string name)
    {
        await LoginAsync().ConfigureAwait(false);
        var url = $"{appSettings.ApiUrl}/recipes";

        CreateRecipeRequest body = new() { Name = name };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
        var slug = await response.Content.ReadFromJsonAsync<string>();
        return slug;
    }

    private async Task UpdateRecipeImage(string slug, string imageUrl)
    {
        await LoginAsync().ConfigureAwait(false);

        var url = $"{appSettings.ApiUrl}/recipes/{slug}/image";

        ImageRequest body = new() { Url = imageUrl };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}