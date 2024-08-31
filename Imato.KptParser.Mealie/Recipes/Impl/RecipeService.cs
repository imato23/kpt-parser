using System.Net.Http.Headers;
using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Common;
using Imato.KptParser.Mealie.Recipes.DomainModel;
using Microsoft.Extensions.Logging;

namespace Imato.KptParser.Mealie.Recipes.Impl;

internal class RecipeService : IRecipeService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<RecipeService> logger;
    private readonly IHelperService helperService;
    private readonly string apiUrl;

    /// <summary>
    ///     Initializes an instance of the RecipeService
    /// </summary>
    public RecipeService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader,
        ILogger<RecipeService> logger, IHelperService helperService)
    {
        AppSettings appSettings = appSettingsReader.GetAppSettings();
        httpClient = httpClientFactory.BuildHttpClient();
        this.logger = logger;
        this.helperService = helperService;
        apiUrl = appSettings.Mealie.ApiUrl;
    }

    public async Task<RecipesResponse> GetAllRecipesAsync()
    {
        string url = $"{apiUrl}/recipes";
        RecipesResponse? recipesResponse =
            await httpClient.GetFromJsonAsync<RecipesResponse>(url).ConfigureAwait(false);

        if (recipesResponse == null)
        {
            throw new InvalidOperationException("Recipes response is null");
        }

        return recipesResponse;
    }

    public async Task<UpdateRecipeRequest?> GetRecipeAsync(string slug)
    {
        string url = $"{apiUrl}/recipes/{slug}";

        string json = await httpClient.GetStringAsync(url).ConfigureAwait(false);

        UpdateRecipeRequest? recipesResponse =
            await httpClient.GetFromJsonAsync<UpdateRecipeRequest>(url).ConfigureAwait(false);

        return recipesResponse;
    }

    public async Task<bool> RecipeExistsAsync(string slug)
    {
        try
        {
            UpdateRecipeRequest? recipe = await GetRecipeAsync(slug).ConfigureAwait(false);

            return recipe != null;
        }
        catch (Exception e)
        {
            // todo: Handle specific exception
            return false;
        }
    }

    public async Task<string> AddRecipeAsync(RecipeRequest recipe)
    {
        string slug = await CreateRecipeAsync(recipe.RecipeName).ConfigureAwait(false);

        await UpdateRecipeImageAsync(slug, recipe.RecipeImageUrl).ConfigureAwait(false);

        return slug;
    }

    public async Task UpdateRecipeAsync(string slug, UpdateRecipeRequest recipe)
    {
        string url = $"{apiUrl}/recipes/{slug}";

        HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, recipe).ConfigureAwait(false);

        await helperService.EnsureSuccessStatusCode(response, $"Recipe for slug {slug} couldn't be updated");
    }

    public async Task UploadImagessForRecipeStepsAsync(string slug, IEnumerable<StepImage> images)
    {
        string url = $"{apiUrl}/recipes/{slug}/assets";

        foreach(StepImage image in images){
            // Download image from url
            byte[] imageBytes = await httpClient.GetByteArrayAsync(image.ImageUrl);

            // Send image to Mealie REST API
            using MultipartFormDataContent formData = new MultipartFormDataContent();
            using ByteArrayContent byteContent = new ByteArrayContent(imageBytes);
            byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            formData.Add(new StringContent(Path.GetFileNameWithoutExtension(image.FileName)), "name");
            formData.Add(new StringContent("mdi-file-image"), "icon");
            formData.Add(new StringContent("jpg"), "extension");
            formData.Add(byteContent, "file", image.FileName);

            HttpResponseMessage response = await httpClient.PostAsync(url, formData);

            await helperService.EnsureSuccessStatusCode(response, $"Asset could not be added to recipe with slug {slug}");
        }
    }

    private async Task<string> CreateRecipeAsync(string name)
    {
        string url = $"{apiUrl}/recipes";

        CreateRecipeRequest body = new CreateRecipeRequest {Name = name};

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        string? slug = await response.Content.ReadFromJsonAsync<string>().ConfigureAwait(false);

        if (slug == null)
        {
            throw new InvalidOperationException("Slug must not be null");
        }

        return slug;
    }

    private async Task UpdateRecipeImageAsync(string slug, string imageUrl)
    {
        string url = $"{apiUrl}/recipes/{slug}/image";

        ImageRequest body = new ImageRequest {Url = imageUrl};

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}
