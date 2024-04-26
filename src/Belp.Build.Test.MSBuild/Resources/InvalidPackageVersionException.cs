namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Occurs when the version of the NuGet package is invalid.
/// </summary>
public class InvalidPackageVersionException : Exception
{
    /// <summary>
    /// Gets the path to the package in question.
    /// </summary>
    public string PackagePath { get; }

    /// <summary>
    /// Gets the invalid version in question.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="InvalidPackageVersionException"/> for the specified <paramref name="packagePath" /> and with the specified <paramref name="version"/>.
    /// </summary>
    /// <param name="packagePath">The path to the package in question.</param>
    /// <param name="version">The invalid version in question.</param>
    public InvalidPackageVersionException(string packagePath, string version)
        : base($"""The package located at {packagePath} has the invalid version "{version}".""")
    {
        PackagePath = packagePath;
        Version = version;
    }
}
