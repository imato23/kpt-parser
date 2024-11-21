using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using System.Net.Http.Json;

namespace Imato.KptParser.Mealie.Categories.Impl;

internal class RecipeCategoryService : IRecipeCategoryService
{
    private readonly string baseUrl;
    private readonly HttpClient httpClient;

    public RecipeCategoryService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        AppSettings appSettings = appSettingsReader.GetAppSettings();
        httpClient = httpClientFactory.BuildHttpClient();
        baseUrl = $"{appSettings.Mealie.ApiUrl}/organizers/categories";
    }

    public async Task<RecipeCategory> GetOrAddCategoryAsync(string name)
    {
        RecipeCategory? recipeCategory = await GetRecipeCategoryAsync(name).ConfigureAwait(false);

        if (recipeCategory != null)
        {
            return recipeCategory;
        }

        return await CreateRecipeCategoryAsync(name).ConfigureAwait(false);
    }

    private async Task<RecipeCategory?> GetRecipeCategoryAsync(string name)
    {
        string url = $"{baseUrl}?queryFilter=name%3D{name}";

        try
        {
            RecipeCategoriesResponse? response =  await httpClient.GetFromJsonAsync<RecipeCategoriesResponse>(url).ConfigureAwait(false);

            if (response != null && response.Items.Any())
            {
                return response.Items.Single();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<RecipeCategory> CreateRecipeCategoryAsync(string name)
    {
        var body = new
        {
            Name = name
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(baseUrl, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RecipeCategory>().ConfigureAwait(false) ?? throw new InvalidOperationException();
    }
}
