using Microsoft.Build.Execution;
using Xunit.Abstractions;
using static Belp.Build.Test.MSBuild.XUnit.Resources.TestSamplesManager;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Provides functions for cloning and using test projects.
/// </summary>
public readonly struct TestProjectInstance
{
    /// <summary>
    /// Gets the original test project.
    /// </summary>
    public TestProject Project { get; }

    /// <summary>
    /// Gets the clone's name.
    /// </summary>
    public string InstanceName { get; }

    /// <summary>
    /// Gets the clone's location.
    /// </summary>
    public string CacheLocation { get; }

    /// <summary>
    /// Gets the logger used by builds.
    /// </summary>
    public ITestOutputHelper Logger { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance"/> with the specified <paramref name="instanceName"/> and the specified <paramref name="logger"/> for the specified <paramref name="project"/>.
    /// </summary>
    /// <param name="instanceName">The clone's name.</param>
    /// <param name="project">The project to based the clone off.</param>
    /// <param name="logger">The logger to be used by builds.</param>
    public TestProjectInstance(string instanceName, TestProject project, ITestOutputHelper logger)
    {
        ArgumentException.ThrowIfNullOrEmpty(instanceName);
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(logger);

        Project = project;
        InstanceName = instanceName;
        CacheLocation = Path.Combine(Paths.ProjectCache, instanceName);
        Logger = logger;
    }

    /// <summary>
    /// Builds the cloned project.
    /// </summary>
    /// <param name="buildParameters">Optional override to the build parameters.</param>
    /// <param name="buildRequestData">Optional override to the build request data.</param>
    /// <returns>The build result.</returns>
    public BuildResult Build(BuildParameters? buildParameters = null, BuildRequestData? buildRequestData = null)
    {
        return BuildManager.DefaultBuildManager.Build(
            buildParameters ?? new BuildParametersWithDefaults(new XUnitMSBuildLoggerAdapter(Logger)),
            buildRequestData ?? new BuildRequestData(
                CacheLocation,
                new Dictionary<string, string>(),
                null,
                ["Restore", "Build"],
                null
            )
        );
    }

    /// <summary>
    /// Builds the cloned project.
    /// </summary>
    /// <param name="buildParameters">Optional override to the build parameters.</param>
    /// <param name="globalProperties"><inheritdoc cref="BuildRequestData(string, IDictionary{string, string}, string, string[], HostServices, BuildRequestDataFlags, RequestedProjectState)" path="/param[@name='globalProperties']" /></param>
    /// <param name="targetsToBuild"><inheritdoc cref="BuildRequestData(string, IDictionary{string, string}, string, string[], HostServices, BuildRequestDataFlags, RequestedProjectState)" path="/param[@name='targetsToBuild']" /></param>
    /// <param name="buildRequestDataFlags"><inheritdoc cref="BuildRequestData(string, IDictionary{string, string}, string, string[], HostServices, BuildRequestDataFlags, RequestedProjectState)" path="/param[@name='buildRequestDataFlags']" /></param>
    /// <param name="toolsVersion"><inheritdoc cref="BuildRequestData(string, IDictionary{string, string}, string, string[], HostServices, BuildRequestDataFlags, RequestedProjectState)" path="/param[@name='toolsVersion']" /></param>
    /// <param name="hostServices"><inheritdoc cref="BuildRequestData(string, IDictionary{string, string}, string, string[], HostServices, BuildRequestDataFlags, RequestedProjectState)" path="/param[@name='hostServices']" /></param>
    /// <returns></returns>
    public BuildResult Build(
        BuildParameters? buildParameters = null,
        Dictionary<string, string>? globalProperties = null,
        string[]? targetsToBuild = null,
        BuildRequestDataFlags buildRequestDataFlags = BuildRequestDataFlags.None,
        string? toolsVersion = null,
        HostServices? hostServices = null
    )
    {
        return BuildManager.DefaultBuildManager.Build(
            buildParameters ?? new BuildParametersWithDefaults(new XUnitMSBuildLoggerAdapter(Logger)),
            new BuildRequestData(
                CacheLocation,
                globalProperties ?? [],
                toolsVersion,
                targetsToBuild ?? ["Restore", "Build"],
                hostServices,
                buildRequestDataFlags
            )
        );
    }

    /// <summary>
    /// Deletes the clone and creates a new clone from <see cref="Project"/>.
    /// </summary>
    /// <returns>This instance.</returns>
    public TestProjectInstance Clone()
    {
        _ = Delete();

        string sourceDirectory = Project.RootPath;

        foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, Path.Combine(CacheLocation, Path.GetRelativePath(sourceDirectory, file)));
        }

        return this;
    }

    /// <summary>
    /// Deletes the clone.
    /// </summary>
    /// <returns>This instance.</returns>
    public TestProjectInstance Delete()
    {
        Directory.Delete(CacheLocation, true);

        return this;
    }
}
