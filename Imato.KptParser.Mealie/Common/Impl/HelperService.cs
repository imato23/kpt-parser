using Microsoft.Extensions.Logging;

namespace Imato.KptParser.Mealie.Common.Impl;

internal class HelperService : IHelperService
{
    private readonly ILogger<HelperService> logger;

    public HelperService(ILogger<HelperService> logger)
    {
        this.logger = logger;
    }

    public async Task EnsureSuccessStatusCode(HttpResponseMessage response, string errorMessage)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        logger.LogCritical("{ErrorMessage}. Error Details: {ErrorDetails}", errorMessage, content);

        throw new InvalidOperationException(errorMessage);
    }
}
