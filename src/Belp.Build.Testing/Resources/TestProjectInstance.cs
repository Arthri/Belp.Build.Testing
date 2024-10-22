﻿using Belp.Build.Testing.Loggers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace Belp.Build.Testing.Resources;

/// <summary>
/// Provides functions for using test projects.
/// </summary>
public abstract class TestProjectInstance
{
    /// <summary>
    /// Gets the test project this instance is based on.
    /// </summary>
    public abstract TestProject TestProject { get; }

    /// <summary>
    /// Gets the MSBuild project used for building.
    /// </summary>
    public abstract Project MSBuildProject { get; }

    private bool _restored;

    /// <summary>
    /// Restores the project instance.
    /// </summary>
    protected internal void Restore()
    {
        var buildParameters = new BuildParameters();
        var buildRequestData = new BuildRequestData(MSBuildProject.CreateProjectInstance(), ["Restore"]);

        _ = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
        _restored = true;
    }

    /// <inheritdoc cref="Build(string[], BuildRequestDataFlags?, HostServices?, Action{BuildParameters}?, Action{BuildRequestData}?, Action{ProjectInstance}?)" />
    public MSBuildResult Build(BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null, Action<ProjectInstance>? configureProjectInstance = null)
    {
        return Build(["Build"], buildRequestDataFlags, hostServices, configureParameters, configureRequestData, configureProjectInstance);
    }

    /// <summary>
    /// Builds the project instance.
    /// </summary>
    /// <param name="targets">The targets to build. <remarks>The restore target is implicitly included.</remarks></param>
    /// <param name="buildRequestDataFlags"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices, BuildRequestDataFlags)" path="/param[@name='flags']" /></param>
    /// <param name="hostServices"><inheritdoc cref="BuildRequestData(ProjectInstance, string[], HostServices)" path="/param[@name='hostServices']" /></param>
    /// <param name="configureParameters">An optional action which configures the assembled <see cref="BuildParameters"/> before building.</param>
    /// <param name="configureRequestData">An optional action which configures the assembled <see cref="BuildRequestData"/> before building.</param>
    /// <param name="configureProjectInstance">An optional action which configures the project instance before building.</param>
    /// <returns>The build result.</returns>
    public MSBuildResult Build(string[] targets, BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null, Action<ProjectInstance>? configureProjectInstance = null)
    {
        var logger = new MSBuildDiagnosticLogger();

        if (!_restored)
        {
            Restore();
        }

        var buildParameters = new BuildParametersWithDefaults(logger);
        MSBuildProject.MarkDirty();
        MSBuildProject.ReevaluateIfNecessary();
        ProjectInstance projectInstance = MSBuildProject.CreateProjectInstance();
        configureProjectInstance?.Invoke(projectInstance);
        var buildRequestData = new BuildRequestData(projectInstance, targets, hostServices, buildRequestDataFlags ?? BuildRequestDataFlags.None);
        configureParameters?.Invoke(buildParameters);
        configureRequestData?.Invoke(buildRequestData);

        BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
        return new MSBuildResult(logger, buildResult, buildResult.ProjectStateAfterBuild);
    }

    /// <summary>
    /// Builds and packs the project instance.
    /// </summary>
    /// <inheritdoc cref="Build(string[], BuildRequestDataFlags?, HostServices?, Action{BuildParameters}?, Action{BuildRequestData}?, Action{ProjectInstance}?)" />
    public MSBuildResult Pack(BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null, Action<ProjectInstance>? configureProjectInstance = null)
    {
        return Build(["Build", "Pack"], buildRequestDataFlags, hostServices, configureParameters, configureRequestData, configureProjectInstance);
    }

    /// <summary>
    /// Builds and publishes the project instance.
    /// </summary>
    /// <inheritdoc cref="Build(string[], BuildRequestDataFlags?, HostServices?, Action{BuildParameters}?, Action{BuildRequestData}?, Action{ProjectInstance}?)" />
    public MSBuildResult Publish(BuildRequestDataFlags? buildRequestDataFlags = null, HostServices? hostServices = null, Action<BuildParameters>? configureParameters = null, Action<BuildRequestData>? configureRequestData = null, Action<ProjectInstance>? configureProjectInstance = null)
    {
        return Build(["Build", "Publish"], buildRequestDataFlags, hostServices, configureParameters, configureRequestData, configureProjectInstance);
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
