namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Occurs when multiple projects inside a sample are matched by the specified pattern.
/// </summary>
public class MultipleProjectsFoundException : Exception
{
    /// <summary>
    /// Gets the name of the searched sample.
    /// </summary>
    public string SampleName { get; }

    /// <summary>
    /// Gets the pattern used to match projects inside the sample.
    /// </summary>
    public string ProjectName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MultipleProjectsFoundException"/> for the specified <paramref name="sampleName"/> and <paramref name="projectName"/>.
    /// </summary>
    /// <param name="sampleName">The name of the searched sample.</param>
    /// <param name="projectName">The pattern used to match projects inside the sample.</param>
    public MultipleProjectsFoundException(string sampleName, string projectName)
        : base("Multiple projects have been found.")
    {
        SampleName = sampleName;
        ProjectName = projectName;
    }
}
