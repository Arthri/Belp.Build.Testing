namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Provides an interface for projects inside test samples.
/// </summary>
public abstract class TestProject
{
    /// <summary>
    /// Gets the name of the project.
    /// </summary>
    /// <remarks>The name includes the project's file extension to avoid ambiguity.</remarks>
    public abstract string Name { get; }

    /// <summary>
    /// Creates a new instance of the current project.
    /// </summary>
    /// <returns>A new instance of the current project.</returns>
    public abstract TestProjectInstance Clone();
}
