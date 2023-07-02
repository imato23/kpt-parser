namespace Imato.KptParser.Common.Config.DomainModel;

/// <summary>
///     The Mealie settings
/// </summary>
public class Mealie
{
    /// <summary>
    ///     The Mealie API url
    /// </summary>
    public required string ApiUrl { get; set; } = null!;

    /// <summary>
    ///     The Mealie username
    /// </summary>
    public required string Username { get; set; } = null!;

    /// <summary>
    ///     The mealie password
    /// </summary>
    public required string Password { get; set; } = null!;
}