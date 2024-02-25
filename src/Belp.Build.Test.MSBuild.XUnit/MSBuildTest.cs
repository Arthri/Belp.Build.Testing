using Belp.Build.Test.MSBuild.XUnit.Resources;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Provides the boilerplate for testing MSBuild projects.
/// </summary>
public class MSBuildTest
{
    /// <summary>
    /// A facade which provides access to the application's resources.
    /// </summary>
    /// <param name="logger">The logger to be used in fetched objects or information.</param>
    public readonly ref struct TestDataFacade(ITestOutputHelper logger)
    {
        /// <summary>
        /// A facade which provides access to the application's sample projects.
        /// </summary>
        /// <param name="logger">The logger to be used in fetched projects.</param>
        public readonly ref struct TestProjectFacade(ITestOutputHelper logger)
        {
            /// <summary>
            /// A facade which provides access to the application's sample projects.
            /// </summary>
            /// <param name="logger">The logger to be used in fetched projects.</param>
            public readonly ref struct ProjectSourceFacade(ITestOutputHelper logger)
            {
                /// <summary>
                /// Fetches the default project of the sample with the specified <paramref name="sampleName"/>.
                /// </summary>
                /// <param name="sampleName">The name of the sample.</param>
                /// <param name="callerMemberName">The name of the caller. <b>Do not specify.</b></param>
                /// <returns>The fetched project.</returns>
                public TestProjectInstance Samples(string sampleName, [CallerMemberName] string? callerMemberName = null)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);
                    ArgumentException.ThrowIfNullOrEmpty(callerMemberName);

                    return TestSamplesManager.TestSamples[sampleName].DefaultProject.Clone(callerMemberName, logger);
                }

                /// <summary>
                /// Fetches a project with the specified <paramref name="projectName"/> from the sample with the specified <paramref name="sampleName"/>.
                /// </summary>
                /// <param name="sampleName">The name of the sample.</param>
                /// <param name="projectName">The name of the project to fetch.</param>
                /// <param name="callerMemberName">The name of the caller. <b>Do not specify.</b></param>
                /// <returns>The fetched project.</returns>
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

            /// <summary>
            /// Gets a facade which fetches projects from the application's resources.
            /// </summary>
            public ProjectSourceFacade From => new(logger);
        }

        /// <summary>
        /// Gets a facade which fetches projects from the application's resources.
        /// </summary>
        public TestProjectFacade Project => new(logger);
    }

    /// <summary>
    /// Gets the logger used by the test.
    /// </summary>
    public ITestOutputHelper Logger { get; }

    /// <summary>
    /// Gets a facade which fetches objects or information from the application's resources.
    /// </summary>
    public TestDataFacade Get => new(Logger);

    /// <summary>
    /// Initializes a new instance of <see cref="MSBuildTest"/> with the specified <paramref name="logger"/>.
    /// </summary>
    /// <param name="logger">The logger to log events to.</param>
    public MSBuildTest(ITestOutputHelper logger)
    {
        Logger = logger;
    }
}
