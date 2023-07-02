namespace Imato.KptParser.Common.Http;

public interface IHttpClientFactory
{
    HttpClient BuildHttpClient();
}