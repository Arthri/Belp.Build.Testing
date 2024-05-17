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
        FileTestProject[] projects = ReadTestProjectsFrom(rootDirectory);
        FileTestProject[]? nestedProjects = null;
        FileTestProject[]? srcProjects = null;

        string samplesDirectoryName = Path.GetFileName(rootDirectory);
        TestProject? defaultProject = FindDefaultProject(samplesDirectoryName, projects);
        string directoryWithSameNameAsParent = Path.Combine(rootDirectory, samplesDirectoryName);
        if (Directory.Exists(directoryWithSameNameAsParent))
        {
            nestedProjects = ReadTestProjectsFrom(directoryWithSameNameAsParent);
            defaultProject ??= FindDefaultProject(samplesDirectoryName, nestedProjects);
        }
        string srcPath = Path.Combine(rootDirectory, "src", samplesDirectoryName);
        if (Directory.Exists(srcPath))
        {
            srcProjects = ReadTestProjectsFrom(srcPath);
            defaultProject ??= FindDefaultProject(samplesDirectoryName, srcProjects);
        }


        int combinedProjectsLength = projects.Length + (nestedProjects?.Length ?? 0) + (srcProjects?.Length ?? 0);
        TestProject[] combinedProjects;
        if (combinedProjectsLength == projects.Length)
        {
            combinedProjects = projects;
        }
        else
        {
            combinedProjects = new TestProject[combinedProjectsLength];
            Span<TestProject> cursor = combinedProjects.AsSpan();
            projects.CopyTo(cursor);
            cursor = cursor[projects.Length..];
            if (nestedProjects is not null)
            {
                nestedProjects.CopyTo(cursor);
                cursor = cursor[nestedProjects.Length..];
            }
            if (srcProjects is not null)
            {
                srcProjects.CopyTo(cursor);
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                cursor = cursor[srcProjects.Length..];
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            }
        }
        return combinedProjects.Length == 0
            ? throw new NoTestProjectsException(rootDirectory)
            : new TestSample(combinedProjects)
            {
                RootPath = rootDirectory,
                // If at least one project is found, then default projects is guaranteed to be non-null
                DefaultProject = defaultProject!,
            };
    }

    private static FileTestProject[] ReadTestProjectsFrom(string path)
    {
        string[] projectPaths = Directory.GetFiles(path, "*.*proj");

        var projects = new FileTestProject[projectPaths.Length];
        for (int i = 0; i < projectPaths.Length; i++)
        {
            projects[i] = new FileTestProject
            {
                Path = projectPaths[i],
            };
        }

        return projects;
    }

    private static TestProject? FindDefaultProject(string directoryName, FileTestProject[] projects)
    {
        IEnumerable<FileTestProject> projectsWithSameNameAsParent = projects.Where(p => Path.GetFileNameWithoutExtension(p.Path) == directoryName);
        using IEnumerator<FileTestProject> enumerator = projectsWithSameNameAsParent.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return projects.OrderBy(static p => p.Path, StringComparer.InvariantCulture).FirstOrDefault();
        }

        TestProject defaultProject = enumerator.Current;

        return enumerator.MoveNext()
            ? throw new MultipleDefaultProjectsFoundException(directoryName, projectsWithSameNameAsParent.Select(p => p.Path))
            : defaultProject;
    }
}
