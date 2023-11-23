namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Represents a sample project used for unit testing.
/// </summary>
public sealed class TestProject
{
    /// <summary>
    /// Gets the root path of the project.
    /// </summary>
    public string RootPath { get; }

    private readonly string[] _projects;

    private readonly string[] _projectsRelative;

    /// <summary>
    /// Gets the projects located inside <see cref="RootPath"/>.
    /// </summary>
    public IReadOnlyList<string> Projects => _projects.AsReadOnly();

    /// <summary>
    /// Gets the relative paths of the projects located inside <see cref="RootPath"/>.
    /// </summary>
    public IReadOnlyList<string> ProjectsRelative => _projectsRelative.AsReadOnly();

    /// <summary>
    /// Gets the project with the same name as <see cref="RootPath"/>.
    /// </summary>
    public string AppellativeProject { get; }

    /// <summary>
    /// Gets the relative path of a project with the same name as <see cref="RootPath"/>.
    /// </summary>
    public string AppellativeProjectRelative { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestProject"/> for the specified <paramref name="rootPath" />.
    /// </summary>
    /// <param name="rootPath">The project's root path.</param>
    /// <exception cref="InvalidOperationException">No projects were found inside <paramref name="rootPath"/>; or zero or more than one appellative projects were found in <paramref name="rootPath"/>.</exception>
    public TestProject(string rootPath)
    {
        RootPath = rootPath;

        string[] projects = Directory.GetFiles(rootPath, "*.*proj");

        if (projects.Length == 0)
        {
            throw new InvalidOperationException($"Directory {rootPath} does not contain any project files.");
        }

        _projects = projects;


        string? projectDirectoryName = Path.GetFileName(Path.GetDirectoryName(rootPath));
        IEnumerable<string> appellativeProjects = projects.Where(p => Path.GetFileNameWithoutExtension(p) == projectDirectoryName);

        IEnumerator<string> e_appellativeProjects = appellativeProjects.GetEnumerator();
        if (!e_appellativeProjects.MoveNext())
        {
            throw new InvalidOperationException($"No appellative projects found in {rootPath}");
        }

        AppellativeProject = e_appellativeProjects.Current;

        if (e_appellativeProjects.MoveNext())
        {
            throw new InvalidOperationException($"More than one appelative projects found in {rootPath}");
        }

        AppellativeProjectRelative = Path.GetRelativePath(rootPath, AppellativeProject);
        _projectsRelative = new string[projects.Length];
        for (int i = 0; i < projects.Length; i++)
        {
            _projectsRelative[i] = Path.GetRelativePath(rootPath, projects[i]);
        }
    }
}
