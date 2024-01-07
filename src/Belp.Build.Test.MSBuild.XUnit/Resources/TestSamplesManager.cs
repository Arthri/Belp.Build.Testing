namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Manages the test projects and test caches.
/// </summary>
public static partial class TestSamplesManager
{
    private static readonly Dictionary<string, TestSample> InternalTestSamples;

    /// <summary>
    /// Gets a dictionary of test projects.
    /// </summary>
    public static IReadOnlyDictionary<string, TestSample> TestSamples => InternalTestSamples.AsReadOnly();

    static TestSamplesManager()
    {
        string[] testSamplesDirectories = Directory.GetDirectories(TestPaths.TestSamples);
        var testSamples = new Dictionary<string, TestSample>(testSamplesDirectories.Length);
        for (int i = 0; i < testSamplesDirectories.Length; i++)
        {
            string testSampleDirectory = testSamplesDirectories[i];
            testSamples[Path.GetFileName(testSampleDirectory)] = TestSample.FromDirectory(testSampleDirectory);
        }

        InternalTestSamples = testSamples;

        ClearCache();
    }

    private static void CreateTempRoot()
    {
        _ = Directory.CreateDirectory(TestPaths.TempRoot);
        File.WriteAllText(
            Path.Combine(TestPaths.TempRoot, "nuget.config"),
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <config>
                <add key="globalPackagesFolder" value="{TestPaths.PackagesCache}" />
              </config>
              <packageSources>
                <clear />
                <add key="Belp.SDK.Test.MSBuild.XUnit Packages" value="{TestPaths.PackagesDirectory}" />
              </packageSources>
            </configuration>
            """
        );

        string packages = string.Join('\n',
            TestPackagesManager
            .Packages
            .Select(static p => $"""    <PackageReference Include="{p.ID}" Version="{p.Version}" />""")
        );
        File.WriteAllText(
            Path.Combine(TestPaths.TempRoot, "Directory.Build.props"),
            $"""
            <Project>

              <ItemGroup>
            {packages}
              </ItemGroup>

              <Import Condition="Exists('$(MSBuildProjectDirectory)\Directory.Test.props')" Project="$(MSBuildProjectDirectory)\Directory.Test.props" />

            </Project>
            """
        );
        File.WriteAllText(
            Path.Combine(TestPaths.TempRoot, "Directory.Build.targets"),
            $"""
            <Project>

              <Import Condition="Exists('$(MSBuildProjectDirectory)\Directory.Test.targets')" Project="$(MSBuildProjectDirectory)\Directory.Test.targets" />

            </Project>
            """
        );
    }

    /// <summary>
    /// Deletes and recreates <see cref="TestPaths.TempRoot"/>.
    /// </summary>
    public static void ClearCache()
    {
        try
        {
            Directory.Delete(TestPaths.TempRoot, true);
        }
        catch (DirectoryNotFoundException)
        {
        }
        CreateTempRoot();
    }

    /// <summary>
    /// Deletes and recreates <see cref="TestPaths.ProjectCache"/>.
    /// </summary>
    public static void ClearProjectsCache()
    {
        try
        {
            Directory.Delete(TestPaths.ProjectCache, true);
        }
        catch (DirectoryNotFoundException)
        {
        }
        _ = Directory.CreateDirectory(TestPaths.ProjectCache);
    }
}
