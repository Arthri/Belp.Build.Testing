using Belp.Build.Test.MSBuild.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

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
    public abstract Project MSBuildProject { get; }

    /// <summary>
    /// Builds the project instance.
    /// </summary>
    /// <param name="buildRequestDataFlags"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices, BuildRequestDataFlags)" path="/param[@name='flags']" /></param>
    /// <param name="hostServices"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices)" path="/param[@name='hostServices']" /></param>
    /// <param name="configureParameters">An optional action which configures the assembled <see cref="BuildParameters"/> before building.</param>
    /// <param name="configureRequestData">An optional action which configures the assembled <see cref="BuildRequestData"/> before building.</param>
    /// <returns>The build result.</returns>
    public MSBuildResult Build(BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null)
    {
        var logger = new MSBuildDiagnosticLogger();

        // Restore
        {
            var buildParameters = new BuildParametersWithDefaults(logger);
            var buildRequestData = new BuildRequestData(MSBuildProject.CreateProjectInstance(), ["Restore"]);

            _ = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
        }

        // Build
        {
            var buildParameters = new BuildParametersWithDefaults(logger);
            MSBuildProject.MarkDirty();
            MSBuildProject.ReevaluateIfNecessary();
            var buildRequestData = new BuildRequestData(MSBuildProject.CreateProjectInstance(), ["Build"], hostServices, buildRequestDataFlags ?? BuildRequestDataFlags.None);
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

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance{T}" /> for the specified <paramref name="project" />.
    /// </summary>
    /// <param name="project">The project which the instance is a clone of.</param>
    public TestProjectInstance(T project)
    {
        TestProject = project;
    }
}
