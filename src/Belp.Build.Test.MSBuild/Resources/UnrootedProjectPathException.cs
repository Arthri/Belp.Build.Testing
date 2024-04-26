namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when the path to a project does not have a root directory.
/// </summary>
public class UnrootedProjectPathException : Exception
{
    /// <summary>
    /// Gets the invalid path in question.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UnrootedProjectPathException"/> for the specified <paramref name="path" />.
    /// </summary>
    /// <param name="path">The invalid path in question.</param>
    public UnrootedProjectPathException(string path)
        : base($"""The project path "{path}" does not contain a root directory.""")
    {
        Path = path;
    }
}
