using Microsoft.Extensions.Logging;
using Slugify;

namespace Imato.KptParser.Mealie.Common.Impl;

internal class HelperService : IHelperService
{
    private readonly ILogger<HelperService> logger;

    public HelperService(ILogger<HelperService> logger)
    {
        this.logger = logger;
    }

    public string Slugify(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        SlugHelperForNonAsciiLanguages slugHelper = new SlugHelperForNonAsciiLanguages();
        return slugHelper.GenerateSlug(input);
    }

    public async Task EnsureSuccessStatusCode(HttpResponseMessage response, string errorMessage)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        else
        {
            string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.LogCritical("{ErrorMessage}. Error Details: {ErrorDetails}", errorMessage, content);

            throw new InvalidOperationException(errorMessage);
        }
    }
}
