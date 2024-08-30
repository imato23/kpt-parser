using System.Net.Http.Json;
using System.Web;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Common;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Imato.KptParser.Mealie.Units.DomainModel;

namespace Imato.KptParser.Mealie.Units.Impl;

internal class UnitService : IUnitService
{
    private readonly string baseUrl;
    private readonly HttpClient httpClient;
    private readonly IHelperService helperService;

    public UnitService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader, IHelperService helperService)
    {
        AppSettings appSettings = appSettingsReader.GetAppSettings();
        httpClient = httpClientFactory.BuildHttpClient();
        baseUrl = $"{appSettings.Mealie.ApiUrl}/units";
        this.helperService = helperService;
    }

    public async Task<Unit> GetOrAddUnitAsync(string name, string abbreviation)
    {
        string plural = string.Empty;

        if (name.EndsWith("(n)"))
        {
            name = name.Remove(name.Length-3);
            abbreviation = abbreviation.Remove(abbreviation.Length-3);
            plural = name + "n";
        }

        ItemResponse<Unit>? unitResponse = await GetAllUnitsAsync(name).ConfigureAwait(false);

        if (unitResponse?.Items == null || !unitResponse.Items.Any())
        {
            return await CreateUnitAsync(name, abbreviation, plural).ConfigureAwait(false);
        }

        if (unitResponse.Items.Count() > 1)
            throw new InvalidOperationException(
                $"There exist multiple units with the name '{name}' in the database. Please cleanup the database");

        return unitResponse.Items.Single();
    }

    private async Task<Unit> CreateUnitAsync(string name, string abbreviation, string plural)
    {
        UnitRequest body = new UnitRequest()
        {
            Name = name,
            Description = string.Empty,
            Abbreviation = abbreviation,
            Fraction = false,
            UseAbbreviation = abbreviation != string.Empty,
            PluralName = plural,
            PluralAbbreviation = plural
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(baseUrl, body).ConfigureAwait(false);

        await helperService.EnsureSuccessStatusCode(response, $"Could not create unit {name}");

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
