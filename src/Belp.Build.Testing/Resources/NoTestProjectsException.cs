namespace Belp.Build.Testing.Resources;

/// <summary>
/// Occurs when no test projects amples are found in a directory.
/// </summary>
public class NoTestProjectsException : Exception
{
    /// <summary>
    /// Gets the path to the directory in question.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="NoTestProjectsException"/> for the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the directory in question.</param>
    public NoTestProjectsException(string path)
        : base($"The directory {path} does not contain test projects.")
    {
        Path = path;
    }
}
