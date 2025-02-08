using Imato.KptParser.Common.Config.DomainModel;
using Imato.KptParser.Common.Config.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Imato.KptParser.Common.Config.Impl;

internal class AppSettingsReader : IAppSettingsReader
{
    private const string SectionName = "App";

    private readonly IConfiguration configuration;

    private AppSettings? appSettings;

    public AppSettingsReader(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public AppSettings GetAppSettings()
    {
        if (appSettings != null) return appSettings;

        IConfigurationSection appConfigSection = configuration.GetSection(SectionName);

        if (!appConfigSection.Exists())
            throw new ConfigReadException(
                $"The configuration section '{SectionName}' does not exist or is empty");

        appSettings = appConfigSection.Get<AppSettings>();

        if (appSettings == null)
            throw new InvalidOperationException("Could not read AppConfig type from configuration file");

        return appSettings;
    }
}