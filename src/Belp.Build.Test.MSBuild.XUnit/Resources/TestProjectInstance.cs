using Microsoft.Build.Execution;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Provides functions for using test projects.
/// </summary>
public abstract class TestProjectInstance
{
    /// <summary>
    /// Gets the original test project.
    /// </summary>
    public abstract TestProject Project { get; }

    private readonly Lazy<ProjectInstance> _projectInstance;

    /// <summary>
    /// Gets the project instance used for building.
    /// </summary>
    public ProjectInstance ProjectInstance => _projectInstance.Value;

    /// <summary>
    /// Gets the name of the clone or instance.
    /// </summary>
    public abstract string InstanceName { get; }

    /// <summary>
    /// Gets the logger used by builds.
    /// </summary>
    public abstract ITestOutputHelper Logger { get; }

    /// <inheritdoc cref="Build(out XUnitMSBuildLoggerAdapter, Action{BuildParameters}?, Action{BuildRequestData}?)" />
    public BuildResult Build(Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null)
    {
        return Build(out _, configureParameters, configureRequestData);
    }

    /// <summary>
    /// Builds the project instance.
    /// </summary>
    /// <param name="logger">The logger used during the build.</param>
    /// <param name="configureParameters">An optional action which configures the assembled <see cref="BuildParameters"/> before building.</param>
    /// <param name="configureRequestData">An optional action which configures the assembled <see cref="BuildRequestData"/> before building.</param>
    /// <returns>The build result.</returns>
    public BuildResult Build(out XUnitMSBuildLoggerAdapter logger, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null)
    {
        var buildParameters = new BuildParametersWithDefaults(logger = new XUnitMSBuildLoggerAdapter(Logger));
        var buildRequestData = new BuildRequestData(ProjectInstance, ["Restore", "Build"]);
        configureParameters?.Invoke(buildParameters);
        configureRequestData?.Invoke(buildRequestData);

        return BuildManager.DefaultBuildManager.Build(
            buildParameters,
            buildRequestData
        );
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance"/>.
    /// </summary>
    public TestProjectInstance()
    {
#pragma warning disable IDE0200 // Remove unnecessary lambda expression
        _projectInstance = new(() => Project.Project.CreateProjectInstance(), true);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression
    }
}

/// <inheritdoc />
/// <typeparam name="T">The type of the test project.</typeparam>
public abstract class TestProjectInstance<T> : TestProjectInstance
    where T : TestProject
{
    /// <inheritdoc />
    public override T TestProject { get; }

    /// <inheritdoc />
    public override string InstanceName { get; }

    /// <inheritdoc />
    public override ITestOutputHelper Logger { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProjectInstance{T}" /> for the specified <paramref name="project" /> with the specified <paramref name="instanceName" /> and <paramref name="logger" />.
    /// </summary>
    /// <param name="project">The project which the instance is a clone of.</param>
    /// <param name="instanceName">The name of the instance.</param>
    /// <param name="logger">The instance's logger.</param>
    public TestProjectInstance(T project, string instanceName, ITestOutputHelper logger)
    {
        Project = project;
        InstanceName = instanceName;
        Logger = logger;
    }
}
