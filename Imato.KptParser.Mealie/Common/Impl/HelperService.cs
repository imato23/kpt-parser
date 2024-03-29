using Slugify;

namespace Imato.KptParser.Mealie.Helper.Impl;

public class HelperService() : IHelperService
{
    public string Slugify(string input)
    {
        SlugHelper slugHelper = new SlugHelper();
        return slugHelper.GenerateSlug(input);
    }
}
