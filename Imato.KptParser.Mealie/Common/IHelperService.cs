namespace Imato.KptParser.Mealie.Common;

public interface IHelperService
{
    string Slugify(string input);

    Task EnsureSuccessStatusCode(HttpResponseMessage response, string errorMessage);
}
