namespace Belp.Build.Test.MSBuild.XUnit;

public record struct TextSpan(TextSpan.Position Start, TextSpan.Position End)
{
    public record struct Position(int Line, int Column);

    public override readonly string ToString()
    {
        return $"{Start.Line}:{Start.Column},${End.Line}:${End.Column}";
    }
}
