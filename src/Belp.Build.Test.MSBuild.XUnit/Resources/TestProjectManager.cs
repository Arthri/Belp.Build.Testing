using System.Security.Cryptography;
using System.Text;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Manages the test projects and test caches.
/// </summary>
public static class TestProjectManager
{
    /// <summary>
    /// Provides common paths used by <see cref="TestProjectManager"/>.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// Gets the directory which contains the test projects.
        /// </summary>
        public static string TestProjectsRoot => AppContext.BaseDirectory;

        /// <summary>
        /// Gets the temporary directory where all caches are located.
        /// </summary>
        public static string TempRoot { get; } = Path.Combine(
            Path.GetTempPath(),
            "23bf55c5-7020-43d0-a313-9695fe6c313b",
            "Belp.SDK.Test.MSBuild.XUnit",
            Convert.ToHexString(SHA256.HashData(Encoding.Unicode.GetBytes(TestProjectsRoot)))
        );

        /// <summary>
        /// Gets the cache directory for cloned projects.
        /// </summary>
        public static string ProjectCache { get; } = Path.Combine(TempRoot, "projects");
    }

    private static readonly Dictionary<string, TestProject> InternalTestProjects;

    /// <summary>
    /// Gets a dictionary of test projects.
    /// </summary>
    public static IReadOnlyDictionary<string, TestProject> TestProjects => InternalTestProjects.AsReadOnly();

    static TestProjectManager()
    {
        if (Paths.TestProjectsRoot is null)
        {
            throw new InvalidProgramException($"{nameof(Paths)}.{nameof(Paths.TestProjectsRoot)} has not been set.");
        }

        string[] testProjectDirectories = Directory.GetDirectories(Paths.TestProjectsRoot);
        var testProjects = new Dictionary<string, TestProject>(testProjectDirectories.Length);
        for (int i = 0; i < testProjectDirectories.Length; i++)
        {
            string testProjectDirectory = testProjectDirectories[i];
            testProjects[Path.GetFileName(testProjectDirectory)] = new TestProject(testProjectDirectory);
        }

        InternalTestProjects = testProjects;
    }

    /// <summary>
    /// Deletes <see cref="Paths.TempRoot"/>.
    /// </summary>
    public static void ClearCache()
    {
        Directory.Delete(Paths.TempRoot, true);
        _ = Directory.CreateDirectory(Paths.TempRoot);
    }
}
