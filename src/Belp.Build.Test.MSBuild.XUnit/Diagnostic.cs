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
        Critical = 1,
        Error = 2,
        Warning = 3,
        Informational = 4,
        Verbose = 5,
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
