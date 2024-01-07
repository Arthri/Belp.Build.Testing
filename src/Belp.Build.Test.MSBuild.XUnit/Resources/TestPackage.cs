namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Represents a NuGet package used in testing.
/// </summary>
internal readonly struct TestPackage
{
    /// <summary>
    /// Gets the package's ID.
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// Gets the package's version.
    /// </summary>
    public string Version { get; }

    public TestPackage(string id, string packageVersion)
    {
        ID = id;
        Version = packageVersion;
    }
}
