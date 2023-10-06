#nullable enable
#if !BELP_BUILD_TEST_MSBUILD_XUNIT_ENABLE_WARNINGS
#pragma warning disable
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

internal static partial class TestProjectManager
{
    public static partial class Paths
    {
#if !BELP_BUILD_TEST_MSBUILD_XUNIT_RESOURCES_PROJECTS_ROOT_SPECIFIED
        public static string ProjectsRoot => throw new NotImplementedException();
#endif

        public static string TempRoot { get; } = Path.Combine(
            Path.GetTempPath(),
            "23bf55c5-7020-43d0-a313-9695fe6c313b",
            "Belp.SDK.Test.MSBuild.XUnit",
            typeof(TestProjectManager).Assembly.GetName().Name ?? throw new InvalidProgramException("Assembly name is null.")
        );

        public static string ProjectCache { get; } = Path.Combine(TempRoot, "projects");
    }

    public static void ClearCache()
    {
        Directory.Delete(Paths.ProjectCache, true);
        _ = Directory.CreateDirectory(Paths.ProjectCache);
    }

    public static string CloneProject(string projectName, [CallerMemberName] string? callerMemberName = null)
    {
        if (callerMemberName is null)
        {
            throw new ArgumentNullException(nameof(callerMemberName));
        }

        string sourceDirectory = Path.Combine(Paths.ProjectsRoot, projectName);
        string destinationDirectory = Path.Combine(Paths.ProjectCache, projectName, callerMemberName);

        foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, Path.Combine(destinationDirectory, Path.GetRelativePath(sourceDirectory, file)));
        }

        return destinationDirectory;
    }
}
