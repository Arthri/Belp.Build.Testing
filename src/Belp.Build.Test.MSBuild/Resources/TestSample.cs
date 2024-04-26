namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Represents a set of data used for testing.
/// </summary>
public sealed class TestSample
{
    /// <summary>
    /// Gets the root path of the sample.
    /// </summary>
    public required string RootPath { get; init; }

    private readonly TestProject[] _testProjects;

    /// <summary>
    /// Gets a list of sample projects.
    /// </summary>
    public IReadOnlyList<TestProject> Projects => _testProjects.AsReadOnly();

    /// <summary>
    /// Gets the default project to be used if no project is specified.
    /// </summary>
    public required TestProject DefaultProject { get; init; }

    private TestSample(TestProject[] testProjects)
    {
        _testProjects = testProjects;
    }

    /// <summary>
    /// Reads a project sample from the specified <paramref name="rootDirectory"/>.
    /// </summary>
    /// <param name="rootDirectory">The directory to read the sample from.</param>
    /// <returns>An object to interface with the project sample.</returns>
    /// <exception cref="InvalidOperationException">No *.*proj files were found.<br />-or-<br />Sample contained multiple projects with the same name as the <paramref name="rootDirectory"/>.</exception>
    public static TestSample FromDirectory(string rootDirectory)
    {
        string[] projectPaths = Directory.GetFiles(rootDirectory, "*.*proj");

        if (projectPaths.Length == 0)
        {
            throw new NoSamplesFoundException(rootDirectory);
        }

        var projects = new FileTestProject[projectPaths.Length];
        for (int i = 0; i < projectPaths.Length; i++)
        {
            projects[i] = new FileTestProject
            {
                Path = projectPaths[i],
            };
        }



        string? samplesDirectoryName = Path.GetFileName(Path.GetDirectoryName(rootDirectory));
        TestProject defaultProject;
        do
        {
            IEnumerable<FileTestProject> projectsWithSameNameAsParent = projects.Where(p => Path.GetFileNameWithoutExtension(p.Path) == samplesDirectoryName);
            using IEnumerator<FileTestProject> enumerator = projectsWithSameNameAsParent.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                defaultProject = projects.OrderBy(static p => p.Path, StringComparer.InvariantCulture).First();
                break;
            }

            defaultProject = enumerator.Current;

            if (enumerator.MoveNext())
            {
                throw new MultipleDefaultProjectsFoundException(rootDirectory, projectsWithSameNameAsParent.Select(p => p.Path));
            }
        }
        while (false);



        return new TestSample(projects)
        {
            RootPath = rootDirectory,
            DefaultProject = defaultProject,
        };
    }
}
