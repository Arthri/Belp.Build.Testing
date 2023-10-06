using Microsoft.Build.Framework;
using System;

namespace Belp.Build.Test.MSBuild.XUnit;

internal record struct Diagnostic(Diagnostic.SeverityLevel Severity, string Code, string? Message, string File, TextSpan Span, string Project)
{
    public enum SeverityLevel
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Informational = 4,
        Verbose = 5,
        Diagnostic = 6,
    }

    public Diagnostic(BuildMessageEventArgs e)
        : this(
            e.Importance switch
            {
                MessageImportance.High => SeverityLevel.Informational,
                MessageImportance.Normal => SeverityLevel.Verbose,
                MessageImportance.Low => SeverityLevel.Diagnostic,
                _ => throw new NotSupportedException(),
            },
            e.Code,
            e.Message,
            e.File,
            new TextSpan(e),
            e.ProjectFile
        )
    {
    }

    public Diagnostic(BuildWarningEventArgs e)
        : this(SeverityLevel.Warning, e.Code, e.Message, e.File, new TextSpan(e), e.ProjectFile)
    {
    }

    public Diagnostic(BuildErrorEventArgs e)
        : this(SeverityLevel.Error, e.Code, e.Message, e.File, new TextSpan(e), e.ProjectFile)
    {
    }

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
