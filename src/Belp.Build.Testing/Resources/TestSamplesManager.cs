using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Belp.Build.Testing.Resources;

/// <summary>
/// Manages the test projects and test caches.
/// </summary>
internal static partial class TestSamplesManager
{
    private static readonly IDictionary<string, TestSample> InternalTestSamples;

    /// <summary>
    /// Gets a dictionary of test projects.
    /// </summary>
    public static IReadOnlyDictionary<string, TestSample> TestSamples => InternalTestSamples.AsReadOnly();

    static TestSamplesManager()
    {
        if (Directory.Exists(TestPaths.TestSamples))
        {
            string[] testSamplesDirectories = Directory.GetDirectories(TestPaths.TestSamples);
            var testSamples = new Dictionary<string, TestSample>(testSamplesDirectories.Length);
            for (int i = 0; i < testSamplesDirectories.Length; i++)
            {
                string testSampleDirectory = testSamplesDirectories[i];
                testSamples[Path.GetFileName(testSampleDirectory)] = TestSample.FromDirectory(testSampleDirectory);
            }

            InternalTestSamples = testSamples;
        }
        else
        {
            InternalTestSamples = ImmutableDictionary<string, TestSample>.Empty;
        }
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
                <add key="Test Packages" value="{TestPaths.PackagesDirectory}" />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
              </packageSources>
            </configuration>
            """
        );

        _ = Directory.CreateDirectory(TestPaths.PackagesDirectory);
        string packages = string.Join('\n',
            TestPackagesManager
            .PackagesList
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
    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "The method should be ran at startup to create the necessary test files.")]
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
