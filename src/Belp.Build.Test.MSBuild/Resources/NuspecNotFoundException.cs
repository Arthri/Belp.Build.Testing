namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when a NuGet package does not contain a NuGet package specification(.nuspec) file.
/// </summary>
public class NuspecNotFoundException : Exception
{
    /// <summary>
    /// Gets the path to the package in question.
    /// </summary>
    public string PackagePath { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="NuspecNotFoundException" /> for the specified <paramref name="packagePath" />.
    /// </summary>
    /// <param name="packagePath">The path to the package in question.</param>
    public NuspecNotFoundException(string packagePath)
        : base($"The package file located at {packagePath} does not contain a .nuspec file.")
    {
        PackagePath = packagePath;
    }
}
