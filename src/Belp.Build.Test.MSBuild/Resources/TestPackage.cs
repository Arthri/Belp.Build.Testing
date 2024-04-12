using IOPath = System.IO.Path;

namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Represents a NuGet package used in testing.
/// </summary>
public readonly struct TestPackage
{
    /// <summary>
    /// Gets the package's ID.
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// Gets the package's version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the path to the package's files.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestPackage"/> with the specified <paramref name="id"/> and <paramref name="packageVersion"/>.
    /// </summary>
    /// <param name="id">The package's ID.</param>
    /// <param name="packageVersion">The package's version.</param>
    public TestPackage(string id, string packageVersion)
    {
        ID = id;
        Version = packageVersion;
        Path = IOPath.Combine(TestPaths.PackagesCache, id.ToLower(), packageVersion);
    }
}
