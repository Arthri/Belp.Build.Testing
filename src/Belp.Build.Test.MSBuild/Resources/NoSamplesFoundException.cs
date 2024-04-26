namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when no samples are found in a directory.
/// </summary>
public class NoSamplesFoundException : Exception
{
    /// <summary>
    /// Gets the path to the directory in question.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="NoSamplesFoundException"/> for the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to the directory in question.</param>
    public NoSamplesFoundException(string path)
        : base($"The directory {path} does not contain samples.")
    {
        Path = path;
    }
}
