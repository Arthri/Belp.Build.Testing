namespace Belp.Build.Test.MSBuild.ObjectModel;

/// <summary>
/// Represents an MSBuild diagnostic.
/// </summary>
/// <param name="Severity">Gets the diagnostic's severity.</param>
/// <param name="Code">Gets the diagnostic's code.</param>
/// <param name="Message">Gets the diagnostic message.</param>
/// <param name="File">Gets the file the diagnostic was raised in.</param>
/// <param name="Span">Gets the location inside <paramref name="File"/> the diagnostic was raised in.</param>
/// <param name="Project">Gets the project the diagnostic was raised in.</param>
public record struct Diagnostic(Diagnostic.SeverityLevel Severity, string Code, string? Message = null, string? File = null, TextSpan Span = default, string? Project = null)
{
    /// <summary>
    /// Represents a diagnostic's severity.
    /// </summary>
    public enum SeverityLevel
    {
        /// <summary>
        /// Events which result in an irrecoverable application state.
        /// </summary>
        Critical = 1,

        /// <summary>
        /// Events which result in termination of some paths due to failure.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Events which are unexpected.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Events which document the state of the application.
        /// </summary>
        Informational = 4,

        /// <summary>
        /// Events which contain more information about the application's current state.
        /// </summary>
        Verbose = 5,

        /// <summary>
        /// Events which contain the most information about the application's current state.
        /// </summary>
        Diagnostic = 6,
    }

    /// <inheritdoc />
    public override readonly string ToString()
    {
        string levelAbbr = Severity switch
        {
            SeverityLevel.Critical => "CRT",
            SeverityLevel.Error => "ERR",
            SeverityLevel.Warning => "WRN",
            SeverityLevel.Informational => "INF",
            SeverityLevel.Verbose => "VRB",
            SeverityLevel.Diagnostic => "DBG",
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
        return new(SeverityLevel.Critical, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new error diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new error diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Error(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(SeverityLevel.Error, code, message, file, span ?? default, project);
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
        return new(SeverityLevel.Warning, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new informational diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new informational diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Info(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(SeverityLevel.Informational, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new verbose diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new verbose diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Verbose(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(SeverityLevel.Verbose, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Creates a new diagnostic-level diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>.
    /// </summary>
    /// <inheritdoc cref="Warn(string, string?, string?, TextSpan?, string?)" path="/param" />
    /// <returns>A new diagnostic-level diagnostic for the specified <paramref name="code"/> with the specified <paramref name="message"/>..</returns>
    public static Diagnostic Diag(string code, string? message, string? file = null, TextSpan? span = null, string? project = null)
    {
        return new(SeverityLevel.Diagnostic, code, message, file, span ?? default, project);
    }

    /// <summary>
    /// Defines an implicit conversion from a tuple containing a severity level and a diagnostic code to a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="tuple">The tuple containing the data.</param>
    public static implicit operator Diagnostic((SeverityLevel Severity, string Code) tuple)
    {
        return new(tuple.Severity, tuple.Code);
    }

    /// <summary>
    /// Defines an implicit conversion from a tuple containing a severity level, a diagnostic code, and a message to a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="tuple">The tuple containing the data.</param>
    public static implicit operator Diagnostic((SeverityLevel Severity, string Code, string Message) tuple)
    {
        return new(tuple.Severity, tuple.Code, tuple.Message);
    }
}
