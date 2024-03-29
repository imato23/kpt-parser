using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Http;

namespace Imato.KptParser.Mealie.Ingredients.Impl
{
    internal class IngredientsService : IIngredientsService
    {
        private readonly AppSettings appSettings;
        private readonly HttpClient httpClient;

        public IngredientsService(IHttpClientFactory httpClientFactory, IAppSettingsReader appSettingsReader)
        {
            appSettings = appSettingsReader.GetAppSettings();
            httpClient = httpClientFactory.BuildHttpClient();
        }


        public Task GetOrAddFoodAsync(string name)
        {
            //httpClient.PostAsync();
            return Task.CompletedTask;
        }
    }
}