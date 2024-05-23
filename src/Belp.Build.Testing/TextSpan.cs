namespace Belp.Build.Testing;

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
    public record struct Position(int Line, int Column)
    {
        /// <summary>
        /// Defines and implicit conversion from an <see cref="int"/>-<see cref="int"/> tuple to a <see cref="Position"/>.
        /// </summary>
        /// <param name="tuple">The tuple to convert to a <see cref="Position"/>.</param>
        public static implicit operator Position((int Line, int Column) tuple)
        {
            return new(tuple.Line, tuple.Column);
        }
    }

    /// <inheritdoc />
    public override readonly string ToString()
    {
        return $"{Start.Line}:{Start.Column},{End.Line}:{End.Column}";
    }

    /// <summary>
    /// Defines and implicit conversion from a <see cref="Position"/>-<see cref="Position"/> tuple to a <see cref="TextSpan"/>.
    /// </summary>
    /// <param name="tuple">The tuple to convert to a <see cref="TextSpan"/>.</param>
    public static implicit operator TextSpan((Position Start, Position End) tuple)
    {
        return new(tuple.Start, tuple.End);
    }
}
