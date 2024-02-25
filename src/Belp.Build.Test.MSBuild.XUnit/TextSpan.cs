namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Represents a portion of text.
/// </summary>
/// <param name="Start">The beginning of the portion.</param>
/// <param name="End">The end of the portion.</param>
public record struct TextSpan(TextSpan.Position Start, TextSpan.Position End)
{
    /// <summary>
    /// Represents a position in text.
    /// </summary>
    /// <param name="Line">The line at which the position begins.</param>
    /// <param name="Column">The column of the specified <paramref name="Line"/> at which the position begins.</param>
    public record struct Position(int Line, int Column);

    /// <inheritdoc />
    public override readonly string ToString()
    {
        return $"{Start.Line}:{Start.Column},${End.Line}:${End.Column}";
    }
}
