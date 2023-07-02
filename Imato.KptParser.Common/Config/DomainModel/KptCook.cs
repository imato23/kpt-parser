namespace Imato.KptParser.Common.Config.DomainModel;

/// <summary>
///     The KptCook settings
/// </summary>
public class KptCook
{
    /// <summary>
    ///     The KptCook API url
    /// </summary>
    public required string ApiUrl { get; set; }

    /// <summary>
    ///     The KptCook API key
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    ///     The KptCook access token
    /// </summary>
    public required string AccessToken { get; set; }
}