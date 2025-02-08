using Imato.KptParser.Common.Config.DomainModel;

namespace Imato.KptParser.Common.Config.Impl;

public class EnvironmentReader : IAppSettingsReader
{
    public AppSettings GetAppSettings()
    {
        AppSettings appSettings = new()
        {
            KptCook = new KptCook
            {
                ApiUrl = GetEnvironmentVariableValue(EnvironmentVariableKey.KPTCOOK_API_URL),
                ApiKey = GetEnvironmentVariableValue(EnvironmentVariableKey.KPTCOOK_API_KEY),
                AccessToken = GetEnvironmentVariableValue(EnvironmentVariableKey.KPTCOOK_ACCESS_TOKEN)
            },
            Mealie = new Mealie
            {
                ApiUrl = GetEnvironmentVariableValue(EnvironmentVariableKey.MEALIE_API_URL),
                Username = GetEnvironmentVariableValue(EnvironmentVariableKey.MEALIE_USERNAME),
                Password = GetEnvironmentVariableValue(EnvironmentVariableKey.MEALIE_PASSWORD),
                CachedIdsFilename = GetEnvironmentVariableValue(EnvironmentVariableKey.MEALIE_CACHED_IDS_FILENAME)
            }
        };

        return appSettings;
    }

    private static string GetEnvironmentVariableValue(EnvironmentVariableKey variableKey)
    {
        string? value = Environment.GetEnvironmentVariable(variableKey.ToString());

        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Environment variable {variableKey} does not exist.");
        }
        
        return value;
    }
}