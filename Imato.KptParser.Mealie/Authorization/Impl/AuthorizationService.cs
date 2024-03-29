using System.Net.Http.Json;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Mealie.Recipes.DomainModel;

namespace Imato.KptParser.Mealie.Authorization.Impl;

internal class AuthorizationService : IAuthorizationService
{
    private readonly HttpClient httpClient;
    private readonly AppSettings appSettings;
    private bool isLoggedIn;

    public AuthorizationService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
    {
        httpClient = httpClientFactory.BuildHttpClient();
        appSettings = appSettingsReader.GetAppSettings();
    }

    public async Task LoginAsync()
    {
        if (!isLoggedIn)
        {
            string accessToken = await FetchAccessTokenAsync().ConfigureAwait(false);
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");
        }

        isLoggedIn = true;
    }

    private async Task<string> FetchAccessTokenAsync()
    {
        var url = $"{appSettings.Mealie.ApiUrl}/auth/token";

        IEnumerable<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("username", appSettings.Mealie.Username),
            new KeyValuePair<string, string>("password", appSettings.Mealie.Password)
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

}