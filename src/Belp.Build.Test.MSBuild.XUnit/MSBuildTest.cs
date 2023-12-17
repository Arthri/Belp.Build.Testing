using Belp.Build.Test.MSBuild.XUnit.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Provides the boilerplate for testing MSBuild projects.
/// </summary>
public class MSBuildTest
{
    public readonly ref struct TestDataFacade(ITestOutputHelper logger)
    {
        public readonly ref struct TestProjectFacade(ITestOutputHelper logger)
        {
            public readonly ref struct ProjectSourceFacade(ITestOutputHelper logger)
            {
                [EditorBrowsable(EditorBrowsableState.Never)]
                public ITestOutputHelper Logger => logger;

                public TestProjectInstance Samples(string sampleName, [CallerMemberName] string? callerMemberName = null)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);
                    ArgumentException.ThrowIfNullOrEmpty(callerMemberName);

                    return new TestProjectInstance(
                        callerMemberName,
                        TestSamplesManager.TestSamples[sampleName].DefaultProject,
                        logger
                    );
                }

                public TestProjectInstance Samples(string sampleName, string projectName, [CallerMemberName] string? callerMemberName = null)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);
                    ArgumentException.ThrowIfNullOrEmpty(projectName);
                    ArgumentException.ThrowIfNullOrEmpty(callerMemberName);

                    TestSample sample = TestSamplesManager.TestSamples[sampleName];
                    TestProject? project = null;
                    if (projectName.Contains('.'))
                    {
                        project = sample.Projects.FirstOrDefault(p => Path.GetFileName(p.Path) == projectName);
                    }
                    if (project is null)
                    {
                        IEnumerable<TestProject> matchingProjects = sample.Projects.Where(p => Path.GetFileNameWithoutExtension(p.Path) == projectName);
                        using IEnumerator<TestProject> enumerator = matchingProjects.GetEnumerator();
                        if (!enumerator.MoveNext())
                        {
                            throw new InvalidOperationException($"Project with the name {projectName} not found.");
                        }

                        project = enumerator.Current;

                        if (enumerator.MoveNext())
                        {
                            throw new InvalidOperationException($"More than one project with the name {projectName}.");
                        }
                    }

                    return new TestProjectInstance(
                        callerMemberName,
                        project,
                        logger
                    );
                }
            }

            public ProjectSourceFacade From => new(logger);
        }

        public TestProjectFacade Project => new(logger);
    }

    public ITestOutputHelper Logger { get; private set; }

    public TestDataFacade Get => new(Logger);

    public MSBuildTest(ITestOutputHelper logger)
    {
        Logger = new XUnitMSBuildLoggerAdapter(logger);
    }

    public MSBuildTest(XUnitMSBuildLoggerAdapter logger)
    {
        Logger = logger;
    }

    public void SetLogger(XUnitMSBuildLoggerAdapter logger)
    {
        Logger = logger;
    }
}
