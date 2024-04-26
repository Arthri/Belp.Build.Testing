namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when a NuGet package specification file(.nuspec) is exceptional.
/// </summary>
public abstract class NuspecException : Exception
{
    /// <summary>
    /// Gets the path to the .nuspec file in question.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="NuspecException" /> for the specified <paramref name="path" /> with the specified <paramref name="message" />.
    /// </summary>
    /// <param name="path">The path to the .nuspec file in question.</param>
    /// <param name="message"><inheritdoc cref="Exception(string?)" path="/param[@name='message']" /></param>
    internal NuspecException(string path, string? message) : base(message)
    {
        Path = path;
    }
}
