using System.Net.Http.Json;
using Imato.KptParser.Mealie.DomainModel;

namespace Imato.KptParser.Mealie.Impl;

public class MealieService: IMealieService
{
    private const string ApiUrl = "https://mealie.home.imato.de/api";
    private const string Username = "thomas";
    private const string Password = "fJGSFCHTQDknS8Bh7aw";

    private readonly HttpClient httpClient;
    
    public MealieService()
    {
        httpClient = new HttpClient();
    }
    
    public async Task LoginAsync()
    {
        string accessToken = await FetchAccessTokenAsync(Username, Password);
        httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");
    }

    public async Task<RecipesResponse?> GetAllRecipes()
    {
        string url = $"{ApiUrl}/recipes";
        RecipesResponse? recipesResponse = await httpClient.GetFromJsonAsync<RecipesResponse>(url).ConfigureAwait(false);
        return recipesResponse;
    }

    private async Task<string> FetchAccessTokenAsync(string username, string password)
    {
        string url = $"{ApiUrl}/auth/token";
        
        IEnumerable<KeyValuePair<string,string>> formData = new List<KeyValuePair<string,string>>
        {
            new("username", Username), 
            new("password", Password)
        };
        
        using (HttpContent formContent = new FormUrlEncodedContent(formData))
        {
            using (HttpResponseMessage response = await httpClient.PostAsync(url, formContent).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                AccessTokenInfo? accessTokenInfo = await response.Content.ReadFromJsonAsync<AccessTokenInfo>().ConfigureAwait(false);
                return accessTokenInfo!.AccessToken;
            }
        }
    }
}