namespace Belp.Build.Testing.Resources;

/// <summary>
/// Represents a cloned instance of <see cref="TestSample"/>.
/// </summary>
public sealed class TestSampleInstance
{
    /// <summary>
    /// Gets the sample this instance is based on.
    /// </summary>
    public TestSample Sample { get; }

    /// <summary>
    /// Gets the directory containing the instance.
    /// </summary>
    public string Directory { get; }

    private readonly FileTestProject.Instance[] _testProjects;

    /// <summary>
    /// Gets a list of cloned test projects.
    /// </summary>
    public IReadOnlyList<FileTestProject.Instance> Projects => _testProjects.AsReadOnly();

    /// <summary>
    /// Gets the default project to be used if no project is specified.
    /// </summary>
    public FileTestProject.Instance DefaultProject { get; }

    internal TestSampleInstance(TestSample sample)
    {
        Sample = sample;

        Directory = TestPaths.GetTempProjectDirectory();
        RecursiveCopy(sample.Directory, Directory);

        DefaultProject = sample.DefaultProject.CloneInto(Path.Combine(Directory, Path.GetRelativePath(sample.Directory, sample.DefaultProject.Directory)));
        IReadOnlyList<FileTestProject> testProjects = sample.Projects;
        var testProjectInstances = new FileTestProject.Instance[testProjects.Count];
        for (int i = 0; i < testProjects.Count; i++)
        {
            FileTestProject testProject = testProjects[i];
            FileTestProject.Instance testProjectInstance =
                ReferenceEquals(sample.DefaultProject, testProject)
                    ? DefaultProject
                    : testProject.CloneInto(Path.Combine(Directory, Path.GetRelativePath(sample.Directory, testProject.Directory)))
                ;
            testProjectInstances[i] = testProjectInstance;
        }
        _testProjects = testProjectInstances;
    }

    private static void RecursiveCopy(string source, string destination)
    {
        var directoriesToCopy = new Queue<string>(System.IO.Directory.GetDirectories(source));

        _ = System.IO.Directory.CreateDirectory(destination);
        foreach (string file in System.IO.Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)));
        }

        while (directoriesToCopy.TryDequeue(out string? directory))
        {
            _ = System.IO.Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, directory)));
            foreach (string file in System.IO.Directory.GetFiles(directory))
            {
                File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)));
            }

            string[] subDirectories = System.IO.Directory.GetDirectories(directory);
            _ = directoriesToCopy.EnsureCapacity(directoriesToCopy.Count + subDirectories.Length);
            foreach (string subDirectory in subDirectories)
            {
                directoriesToCopy.Enqueue(subDirectory);
            }
        }
    }
}
