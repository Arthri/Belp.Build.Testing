namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when multiple default projects are found while reading a directory for samples.
/// </summary>
public class MultipleDefaultProjectsFoundException : Exception
{
    /// <summary>
    /// Gets the path to the directory in question.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a list of projects that can be considered default projects.
    /// </summary>
    public IReadOnlyList<string> Projects { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MultipleDefaultProjectsFoundException"/> for the specified <paramref name="path"/> and with the specified <paramref name="projects"/>.
    /// </summary>
    /// <param name="path">The path to the directory in question.</param>
    /// <param name="projects">The list of possible default projects considered.</param>
    public MultipleDefaultProjectsFoundException(string path, IEnumerable<string> projects)
        : base($"The directory {path} contains multiple possible default projects.")
    {
        Path = path;
        Projects = new List<string>(projects).AsReadOnly();
    }
}
