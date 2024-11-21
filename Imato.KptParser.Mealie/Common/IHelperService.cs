namespace Imato.KptParser.Mealie.Common;

public interface IHelperService
{
    Task EnsureSuccessStatusCode(HttpResponseMessage response, string errorMessage);
}
