using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Web;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Common;
using Imato.KptParser.Mealie.Foods.DomainModel;
using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Foods.Impl;

internal class FoodService : IFoodService
{
    private readonly string baseUrl;
    private readonly HttpClient httpClient;
    private readonly IHelperService helperService;

    public FoodService(
        IHttpClientFactory httpClientFactory,
        IAppSettingsReader appSettingsReader,
        IHelperService helperService)
    {
        AppSettings appSettings = appSettingsReader.GetAppSettings();
        httpClient = httpClientFactory.BuildHttpClient();
        baseUrl = $"{appSettings.Mealie.ApiUrl}/foods";
        this.helperService = helperService;
    }

    public async Task<Food> GetOrAddFoodAsync(string name)
    {
        Regex regex = new Regex(@"\s*\([^)]*\)");
        string nameWithoutBrackets = regex.Replace(name, string.Empty).Trim();

        ItemResponse<Food>? foodResponse = await GetAllFoodsAsync(nameWithoutBrackets).ConfigureAwait(false);

        if (foodResponse?.Items == null || !foodResponse.Items.Any())
        {
            return await CreateFoodAsync(nameWithoutBrackets).ConfigureAwait(false);
        }

        if (foodResponse.Items.Count() > 1)
            throw new InvalidOperationException(
                $"There exist multiple foods with the name '{nameWithoutBrackets}' in the database. Please cleanup the database");

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

        helperService.EnsureSuccessStatusCode(response, $"Couldn't create food '{name}'");

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
