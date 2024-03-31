using Microsoft.Extensions.Logging;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Represents an MSBuild diagnostic.
/// </summary>
/// <param name="Severity">Gets the diagnostic's severity.</param>
/// <param name="Code">Gets the diagnostic's code.</param>
/// <param name="Message">Gets the diagnostic message.</param>
/// <param name="File">Gets the file the diagnostic was raised in.</param>
/// <param name="Span">Gets the location inside <paramref name="File"/> the diagnostic was raised in.</param>
/// <param name="Project">Gets the project the diagnostic was raised in.</param>
public record struct Diagnostic(LogLevel Severity, string Code, string? Message = null, string? File = null, TextSpan Span = default, string? Project = null)
{
    /// <inheritdoc />
    public override readonly string ToString()
    {
        string levelAbbr = Severity switch
        {
            LogLevel.Critical => "CRT",
            LogLevel.Error => "ERR",
            LogLevel.Warning => "WRN",
            LogLevel.Information => "INF",
            LogLevel.Debug => "DBG",
            LogLevel.Trace => "TRC",
            LogLevel.None => "NON",
            _ => throw new NotSupportedException(),
        };
        return $"[{levelAbbr}] {Code}{(Message is null ? "" : $": {Message}")} @ {File}({Span}) [{Project}]";
    }

    /// <summary>
    /// Creates a new critical diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new critical diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Critical(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Critical, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new error diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new error diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Error(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Error, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new warning for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <param name="code">The diagnostic's code.</param>
    /// <param name="message">The diagnostic's message.</param>
    /// <param name="file">The file the diagnostic was raised in.</param>
    /// <param name="span">The location inside <paramref name="file"/> the diagnostic was raised in.</param>
    /// <param name="project">The project the diagnostic was raised in.</param>
    /// <returns>A new warning diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Warn(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Warning, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new informational diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new informational diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Info(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Information, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new debug diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new debug diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Debug(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Debug, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new trace diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new trace diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Trace(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(LogLevel.Trace, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Defines an implicit conversion from a tuple containing a severity level and a diagnostic code to a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="tuple">The tuple containing the data.</param>
    public static implicit operator Diagnostic((LogLevel Severity, string Code) tuple)
    {
        return new(tuple.Severity, tuple.Code);
    }

    /// <summary>
    /// Defines an implicit conversion from a tuple containing a severity level, a diagnostic code, and a message to a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="tuple">The tuple containing the data.</param>
    public static implicit operator Diagnostic((LogLevel Severity, string Code, string Message) tuple)
    {
        return new(tuple.Severity, tuple.Code, tuple.Message);
    }
}
