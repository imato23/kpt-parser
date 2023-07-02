using Imato.KptParser.Common.Config.DomainModel;

namespace Imato.KptParser.Common.Config;

public interface IAppSettingsReader
{ 
    Task<AppSettings> GetAppSettingsAsync();
    AppSettings GetAppSettings();
}