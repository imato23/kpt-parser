using System.Net.Http.Json;
using System.Web;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Imato.KptParser.Mealie.Units.DomainModel;

namespace Imato.KptParser.Mealie.Units.Impl;

internal class UnitService : IUnitService
{
    private readonly string baseUrl;
    private readonly HttpClient httpClient;

    public UnitService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        Common.Config.DomainModel.Mealie appSettings = appSettingsReader.GetAppSettings().Mealie;
        httpClient = httpClientFactory.BuildHttpClient();
        baseUrl = $"{appSettings.ApiUrl}/units";
    }
    
    public async Task<Unit> GetOrAddUnitAsync(string name, string abbreviation)
    {
        ItemResponse<Unit>? unitResponse = await GetAllUnitsAsync(name).ConfigureAwait(false);

        if (unitResponse?.Items == null || !unitResponse.Items.Any())
        {
            return await CreateUnitAsync(name, abbreviation).ConfigureAwait(false);
        }

        if (unitResponse.Items.Count() > 1)
            throw new InvalidOperationException(
                $"There exist multiple units with the name '{name}' in the database. Please cleanup the database");

        return unitResponse.Items.Single();
    }

    private async Task<Unit> CreateUnitAsync(string name, string abbreviation)
    {
        UnitRequest body = new UnitRequest()
        {
            Name = name,
            Description = string.Empty,
            Abbreviation = abbreviation,
            Fraction = false,
            UseAbbreviation = abbreviation != string.Empty,
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(baseUrl, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<Unit>().ConfigureAwait(false) ?? throw new InvalidOperationException();
    }

    private async Task<ItemResponse<Unit>?> GetAllUnitsAsync(string? name = null)
    {
        string url = baseUrl;

        string queryFilter = HttpUtility.UrlEncode($"name={name}");

        if (name != null)
        {
            url += $"?queryFilter={queryFilter}";
        }

        return await httpClient.GetFromJsonAsync<ItemResponse<Unit>>(url).ConfigureAwait(false);
    }
}