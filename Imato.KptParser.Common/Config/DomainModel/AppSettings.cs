namespace Imato.KptParser.Common.Config.DomainModel;

/// <summary>
///     The application settings
/// </summary>
public class AppSettings
{
    /// <summary>
    ///     The KptCook settings
    /// </summary>
    public required KptCook KptCook { get; set; }

    /// <summary>
    ///     The Mealie settings
    /// </summary>
    public required Mealie Mealie { get; set; }
}