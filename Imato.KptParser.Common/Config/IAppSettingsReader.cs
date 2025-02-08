using Imato.KptParser.Common.Config.DomainModel;

namespace Imato.KptParser.Common.Config;

public interface IAppSettingsReader
{
    AppSettings GetAppSettings();
}