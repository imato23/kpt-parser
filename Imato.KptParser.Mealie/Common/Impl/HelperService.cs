using Slugify;

namespace Imato.KptParser.Mealie.Common.Impl;

public class HelperService() : IHelperService
{
    public string Slugify(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        SlugHelper slugHelper = new SlugHelper();
        return slugHelper.GenerateSlug(input);
    }
}
