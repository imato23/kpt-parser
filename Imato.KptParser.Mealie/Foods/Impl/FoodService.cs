using System.Net.Http.Json;
using System.Web;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Foods.DomainModel;
using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Foods.Impl;

internal class FoodService : IFoodService
{
    private readonly string baseUrl;
    private readonly HttpClient httpClient;

    public FoodService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        AppSettings appSettings = appSettingsReader.GetAppSettings();
        httpClient = httpClientFactory.BuildHttpClient();
        baseUrl = $"{appSettings.Mealie.ApiUrl}/foods";
    }

    public async Task<Food> GetOrAddFoodAsync(string name)
    {
        ItemResponse<Food>? foodResponse = await GetAllFoodsAsync(name).ConfigureAwait(false);

        if (foodResponse?.Items == null || !foodResponse.Items.Any())
        {
            return await CreateFoodAsync(name).ConfigureAwait(false);
        }

        if (foodResponse.Items.Count() > 1)
            throw new InvalidOperationException(
                $"There exist multiple foods with the name '{name}' in the database. Please cleanup the database");

        return foodResponse.Items.Single();
    }

    private async Task<Food> CreateFoodAsync(string name)
    {
        FoodRequest body = new FoodRequest
        {
            Name = name,
            Description = string.Empty,
            LabelId = Guid.NewGuid().ToString()
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(baseUrl, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Food>().ConfigureAwait(false) ?? throw new InvalidOperationException();
    }
    
    private async Task<ItemResponse<Food>?> GetAllFoodsAsync(string? name = null)
    {
        string url = baseUrl;

        string queryFilter = HttpUtility.UrlEncode($"name={name}");

        if (name != null) url += $"?queryFilter={queryFilter}";

        return await httpClient.GetFromJsonAsync<ItemResponse<Food>>(url).ConfigureAwait(false);
    }
}