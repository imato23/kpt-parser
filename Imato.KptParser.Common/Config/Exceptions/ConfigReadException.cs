namespace Imato.KptParser.Common.Config.Exceptions;

/// <summary>
///     The config read exception
/// </summary>
public class ConfigReadException : Exception
{
    public ConfigReadException()
    {
    }

    public ConfigReadException(string? message)
        : base(message)
    {
    }

    public ConfigReadException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}