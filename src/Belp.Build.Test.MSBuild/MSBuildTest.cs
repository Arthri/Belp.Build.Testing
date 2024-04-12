using Belp.Build.Test.MSBuild.Resources;
using System.Diagnostics.CodeAnalysis;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Provides the boilerplate for testing MSBuild projects.
/// </summary>
public class MSBuildTest
{
    /// <summary>
    /// Provides a facade which initiates a request to load something.
    /// </summary>
    public readonly ref struct LoadFacade
    {
        /// <summary>
        /// Provides a facade which initiates a request to load a project.
        /// </summary>
        public readonly ref struct ProjectFacade
        {
            /// <summary>
            /// Provides a facade which initiates a request to load a project from somewhere.
            /// </summary>
            public readonly ref struct FromFacade
            {
                /// <summary>
                /// Fetches the default project of the sample with the specified <paramref name="sampleName"/>.
                /// </summary>
                /// <param name="sampleName">The name of the sample.</param>
                /// <returns>The fetched project.</returns>
                [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The member is intentionally declared on the instance so as to facilitate the fluent API.")]
                public TestProjectInstance Samples(string sampleName)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);

                    return TestSamplesManager.TestSamples[sampleName].DefaultProject.Clone();
                }

                /// <summary>
                /// Fetches a project with the specified <paramref name="projectName"/> from the sample with the specified <paramref name="sampleName"/>.
                /// </summary>
                /// <param name="sampleName">The name of the sample.</param>
                /// <param name="projectName">The name of the project to fetch.</param>
                /// <returns>The fetched project.</returns>
                [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The member is intentionally declared on the instance so as to facilitate the fluent API.")]
                public TestProjectInstance Samples(string sampleName, string projectName)
                {
                    ArgumentException.ThrowIfNullOrEmpty(sampleName);
                    ArgumentException.ThrowIfNullOrEmpty(projectName);

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
                        : project.Clone()
                        ;
                }
            }

            /// <summary>
            /// Gets a facade which initiates a request to load a project from somewhere.
            /// </summary>
            [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The member is intentionally declared on the instance so as to facilitate the fluent API.")]
            public FromFacade From => new();
        }

        /// <summary>
        /// Gets a facade which initiates a request to load a project.
        /// </summary>
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The member is intentionally declared on the instance so as to facilitate the fluent API.")]
        public ProjectFacade Project => new();
    }

    /// <summary>
    /// Gets a facade which initiates a request to load something.
    /// </summary>
    public static LoadFacade Load => new();
}
