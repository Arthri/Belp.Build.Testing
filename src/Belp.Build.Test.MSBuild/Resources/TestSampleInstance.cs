namespace Belp.Build.Test.MSBuild.Resources;

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
    /// Gets the location of the instance's cloned files.
    /// </summary>
    public string RootPath { get; }

    private readonly FileTestProject.Instance[] _testProjects;

    /// <summary>
    /// Gets a list of sample project instances.
    /// </summary>
    public IReadOnlyList<FileTestProject.Instance> Projects => _testProjects.AsReadOnly();

    /// <summary>
    /// Gets the default project instance to be used if no project is specified.
    /// </summary>
    public FileTestProject.Instance DefaultProject { get; }

    internal TestSampleInstance(TestSample sample)
    {
        Sample = sample;

        RootPath = Path.Combine(TestPaths.ProjectCache, Guid.NewGuid().ToString("N"));
        RecursiveCopy(sample.RootPath, RootPath);

        DefaultProject = sample.DefaultProject.CloneInto(Path.Combine(RootPath, Path.GetRelativePath(sample.RootPath, sample.DefaultProject.RootPath)));
        IReadOnlyList<FileTestProject> testProjects = sample.Projects;
        var testProjectInstances = new FileTestProject.Instance[testProjects.Count];
        for (int i = 0; i < testProjects.Count; i++)
        {
            FileTestProject testProject = testProjects[i];
            FileTestProject.Instance testProjectInstance =
                ReferenceEquals(sample.DefaultProject, testProject)
                    ? DefaultProject
                    : testProject.CloneInto(Path.Combine(RootPath, Path.GetRelativePath(sample.RootPath, testProject.RootPath)))
                ;
            testProjectInstances[i] = testProjectInstance;
        }
        _testProjects = testProjectInstances;
    }

    private static void RecursiveCopy(string source, string destination)
    {
        var directoriesToCopy = new Queue<string>(Directory.GetDirectories(source));

        _ = Directory.CreateDirectory(destination);
        foreach (string file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)));
        }

        while (directoriesToCopy.TryDequeue(out string? directory))
        {
            _ = Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, directory)));
            foreach (string file in Directory.GetFiles(directory))
            {
                File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)));
            }

            string[] subDirectories = Directory.GetDirectories(directory);
            _ = directoriesToCopy.EnsureCapacity(directoriesToCopy.Count + subDirectories.Length);
            foreach (string subDirectory in subDirectories)
            {
                directoriesToCopy.Enqueue(subDirectory);
            }
        }
    }
}
