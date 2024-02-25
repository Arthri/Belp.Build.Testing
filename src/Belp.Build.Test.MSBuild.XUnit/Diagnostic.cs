namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Represents an MSBuild diagnostic.
/// </summary>
/// <param name="Severity">Gets the diagnostic's severity.</param>
/// <param name="Code">Gets the diagnostic's code.</param>
/// <param name="Message">Gets the diagnostic message.</param>
/// <param name="File">Gets the file the diagnostic was raised in.</param>
/// <param name="Span">Gets the location inside <paramref name="File"/> the diagnostic was raised in.</param>
/// <param name="Project">Gets the project the diagnostic was raised in.</param>
public record struct Diagnostic(Diagnostic.SeverityLevel Severity, string Code, string? Message, string File, TextSpan Span, string Project)
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
}
