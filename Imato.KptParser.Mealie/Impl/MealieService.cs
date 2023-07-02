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

    public async Task AddRecipe(RecipeRequest recipe)
    {
        string? slug = await CreateRecipeAsync(recipe.RecipeName).ConfigureAwait(false);

        if (slug == null)
        {
            throw new InvalidOperationException("Couldn't create recipe");
        }
        
        await UpdateRecipeImage(slug, recipe.RecipeImageUrl);
    }
    
    private async Task<string?> CreateRecipeAsync(string name)
    {
        await LoginAsync().ConfigureAwait(false);
        string url = $"{appSettings.ApiUrl}/recipes";

        CreateRecipeRequest body = new CreateRecipeRequest { Name = name };
        
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
        string? slug = await response.Content.ReadFromJsonAsync<string>();
        return slug;
    }

    private async Task UpdateRecipeImage(string slug, string imageUrl)
    {
        string url = $"{appSettings.ApiUrl}/recipes/{slug}/image";

        ImageRequest body = new ImageRequest { Url = imageUrl };
        
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    private async Task UpdateRecipe(string slug)
    {
        
    }
}