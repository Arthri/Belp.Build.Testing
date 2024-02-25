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

                    return TestSamplesManager.TestSamples[sampleName].DefaultProject.Clone(callerMemberName, logger);
                }

                public TestProjectInstance Samples(string sampleName, string projectName, [CallerMemberName] string? callerMemberName = null)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);
                    ArgumentException.ThrowIfNullOrEmpty(projectName);
                    ArgumentException.ThrowIfNullOrEmpty(callerMemberName);

                    TestSample sample = TestSamplesManager.TestSamples[sampleName];
                    TestProject? project = null;
                    IEnumerable<TestProject> matchingProjects = sample.Projects.Where(p => p.Name.StartsWith(projectName));
                    using IEnumerator<TestProject> enumerator = matchingProjects.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        throw new InvalidOperationException($"Project with the name {projectName} not found.");
                    }

                    project = enumerator.Current;

                    return enumerator.MoveNext()
                        ? throw new InvalidOperationException($"More than one project with the name {projectName}.")
                        : project.Clone(callerMemberName, logger)
                        ;
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
