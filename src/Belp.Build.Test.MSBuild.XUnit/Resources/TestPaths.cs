namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Provides common paths used by <see cref="TestSamplesManager"/>.
/// </summary>
public static class TestPaths
{
    /// <summary>
    /// Gets the directory which contains the test files.
    /// </summary>
    public static string TestRoot => AppContext.BaseDirectory;

    /// <summary>
    /// Gets the directory which contains the test projects.
    /// </summary>
    public static string TestSamples => Path.Combine(
        TestRoot,
        "samples"
    );

    /// <summary>
    /// Gets the directory which contains the packages to be tested.
    /// </summary>
    public static string TestPackages => Path.Combine(
        TestRoot,
        "packages"
    );

    /// <summary>
    /// Gets the temporary directory where all caches are located.
    /// </summary>
    public static string TempRoot { get; } = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid().ToString("N")
    );

    /// <summary>
    /// Gets the cache directory for cloned projects.
    /// </summary>
    public static string ProjectCache { get; } = Path.Combine(TempRoot, "projects");

    /// <summary>
    /// Gets the temporary packages source.
    /// </summary>
    public static string PackagesDirectory { get; } = TestPackages;

    /// <summary>
    /// Gets the cache directory for restored packages.
    /// </summary>
    public static string PackagesCache { get; } = Path.Combine(TempRoot, "packages_cache");
}
