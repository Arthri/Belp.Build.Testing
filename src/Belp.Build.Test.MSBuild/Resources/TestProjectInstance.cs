using Belp.Build.Test.MSBuild.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Provides functions for using test projects.
/// </summary>
public abstract class TestProjectInstance
{
    /// <summary>
    /// Gets the original test project.
    /// </summary>
    public abstract TestProject TestProject { get; }

    /// <summary>
    /// Gets the MSBuild project used for building.
    /// </summary>
    public abstract Project Project { get; }

    /// <summary>
    /// Gets the logger used by builds.
    /// </summary>
    public abstract ITestOutputHelper Logger { get; }

    /// <inheritdoc cref="Build(out XUnitLogger, BuildRequestDataFlags?, HostServices?, Action{BuildParameters}?, Action{BuildRequestData}?)" />
    public MSBuildResult Build(BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null)
    {
        return Build(out _, buildRequestDataFlags, hostServices, configureParameters, configureRequestData);
    }

    /// <summary>
    /// Builds the project instance.
    /// </summary>
    /// <param name="logger">The logger used during the build.</param>
    /// <param name="buildRequestDataFlags"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices, BuildRequestDataFlags)" path="/param[@name='flags']" /></param>
    /// <param name="hostServices"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices)" path="/param[@name='hostServices']" /></param>
    /// <param name="configureParameters">An optional action which configures the assembled <see cref="BuildParameters"/> before building.</param>
    /// <param name="configureRequestData">An optional action which configures the assembled <see cref="BuildRequestData"/> before building.</param>
    /// <returns>The build result.</returns>
    public MSBuildResult Build(out XUnitLogger logger, BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null)
    {
        // Restore
        {
            var buildParameters = new BuildParametersWithDefaults(new XUnitLogger(Logger));
            var buildRequestData = new BuildRequestData(Project.CreateProjectInstance(), ["Restore"]);

            _ = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
        }

        // Build
        {
            var buildParameters = new BuildParametersWithDefaults(logger = new XUnitLogger(Logger));
            Project.MarkDirty();
            Project.ReevaluateIfNecessary();
            var buildRequestData = new BuildRequestData(Project.CreateProjectInstance(), ["Build"], hostServices, buildRequestDataFlags ?? BuildRequestDataFlags.None);
            configureParameters?.Invoke(buildParameters);
            configureRequestData?.Invoke(buildRequestData);

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
            return new MSBuildResult(logger, buildResult, buildResult.ProjectStateAfterBuild);
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance"/>.
    /// </summary>
    public TestProjectInstance()
    {
    }
}

/// <inheritdoc />
/// <typeparam name="T">The type of the test project.</typeparam>
public abstract class TestProjectInstance<T> : TestProjectInstance
    where T : TestProject
{
    /// <inheritdoc />
    public sealed override T TestProject { get; }

    /// <inheritdoc />
    public sealed override ITestOutputHelper Logger { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance{T}" /> for the specified <paramref name="project" /> with the specified <paramref name="logger" />.
    /// </summary>
    /// <param name="project">The project which the instance is a clone of.</param>
    /// <param name="logger">The instance's logger.</param>
    public TestProjectInstance(T project, ITestOutputHelper logger)
    {
        TestProject = project;
        Logger = logger;
    }
}
