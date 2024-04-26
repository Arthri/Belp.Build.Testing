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

    /// <summary>
    /// Gets the path to a file in the package.
    /// </summary>
    /// <returns>The path to a file in the package.</returns>
    public string GetPath(string path1)
    {
        return IOPath.Combine(Path, path1);
    }

    /// <summary>
    /// Gets the path to a file in the package.
    /// </summary>
    /// <param name="path1">The first path to combine.</param>
    /// <param name="path2">The second path to combine.</param>
    /// <returns>The path to a file in the package.</returns>
    public string GetPath(string path1, string path2)
    {
        return IOPath.Combine(Path, path1, path2);
    }

    /// <summary>
    /// Gets the path to a file in the package.
    /// </summary>
    /// <param name="path1">The first path to combine.</param>
    /// <param name="path2">The second path to combine.</param>
    /// <param name="path3">The third path to combine.</param>
    /// <returns>The path to a file in the package.</returns>
    public string GetPath(string path1, string path2, string path3)
    {
        return IOPath.Combine(Path, path1, path2, path3);
    }

    /// <summary>
    /// Gets the path to a file in the package.
    /// </summary>
    /// <param name="paths">An array of paths to combine.</param>
    /// <returns>The path to a file in the package.</returns>
    public string GetPath(params string[] paths)
    {
        string[] pathsToCombine = new string[paths.Length + 1];
        paths.CopyTo(pathsToCombine.AsSpan(1));
        pathsToCombine[0] = Path;
        return IOPath.Combine(paths);
    }
}
