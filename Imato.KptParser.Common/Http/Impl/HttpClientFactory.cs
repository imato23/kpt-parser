namespace Imato.KptParser.Common.Http.Impl;

/// <summary>
///     Http client factory
/// </summary>
internal class HttpClientFactory : IHttpClientFactory, IDisposable
{
    private HttpClient? httpClient;

    public void Dispose()
    {
        httpClient?.Dispose();
    }

    /// <summary>
    ///     Builds a http client instance.
    /// </summary>
    /// <returns>The http client instance.</returns>
    public HttpClient BuildHttpClient()
    {
        return httpClient ??= new HttpClient();
    }
}