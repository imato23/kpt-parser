using System.Runtime.Serialization;

namespace Imato.KptParser.Common.Config.Exceptions;

/// <summary>
/// The config read exception
/// </summary>
public class ConfigReadException : Exception
{
    public ConfigReadException()
    {
    }

    protected ConfigReadException(SerializationInfo info, StreamingContext context)
        : base(info, context)
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