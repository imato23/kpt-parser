using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Http;

namespace Imato.KptParser.Mealie.Impl
{
    internal class IngredientsService : IIngredientsService
    {
        private readonly Common.Config.DomainModel.Mealie appSettings;
        private readonly HttpClient httpClient;

        public IngredientsService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
        {
            appSettings = appSettingsReader.GetAppSettings().Mealie;
            httpClient = httpClientFactory.BuildHttpClient();
        }


        public Task GetOrAddFoodAsync(string name)
        {
            httpClient.PostAsync();
        }
    }
}