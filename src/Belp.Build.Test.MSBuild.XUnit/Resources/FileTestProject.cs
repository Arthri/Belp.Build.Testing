using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using IOPath = System.IO.Path;
using MSBuildProject = Microsoft.Build.Evaluation.Project;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Represents a <see cref="TestProject"/> backed by a file.
/// </summary>
public sealed class FileTestProject : TestProject
{
    /// <summary>
    /// Represents a test project instance for <see cref="FileTestProject"/>.
    /// </summary>
    /// <remarks>Instances must be created using <see cref="FileTestProject.Clone(string, ITestOutputHelper)"/>.</remarks>
    public sealed class Instance : TestProjectInstance<FileTestProject>
    {
        /// <summary>
        /// Gets the clone's location.
        /// </summary>
        public string CacheLocation { get; }

        private readonly Lazy<MSBuildProject> _project;

        /// <inheritdoc />
        public override MSBuildProject Project => _project.Value;

        internal Instance(FileTestProject project, string instanceName, ITestOutputHelper logger)
            : base(project, instanceName, logger)
        {
            CacheLocation = IOPath.Combine(TestPaths.ProjectCache, instanceName);
            _project = new(() => MSBuildProject.FromFile(IOPath.Combine(CacheLocation, IOPath.GetRelativePath(TestProject.RootPath, TestProject.Path)), new()), true);

            if (!Directory.Exists(CacheLocation))
            {
                Clone();
            }
        }

        /// <summary>
        /// Deletes the cloned files.
        /// </summary>
        public void Delete()
        {
            try
            {
                Directory.Delete(CacheLocation, true);
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        /// <summary>
        /// Deletes the previously cloned files and clones the project once more.
        /// </summary>
        public void Clone()
        {
            Delete();

            string sourceDirectory = TestProject.RootPath;

            foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string destinationPath = IOPath.Combine(CacheLocation, IOPath.GetRelativePath(sourceDirectory, file));
                string? destinationDirectory = IOPath.GetDirectoryName(destinationPath);
                if (destinationDirectory is not null)
                {
                    _ = Directory.CreateDirectory(destinationDirectory);
                }
                File.Copy(file, destinationPath);
            }
        }
    }

    /// <summary>
    /// Gets the path the project is located in.
    /// </summary>
    public string RootPath { get; private init; }

    private readonly string _path;

    /// <summary>
    /// Gets the path to the project file.
    /// </summary>
    public required string Path
    {
        get => _path;

        [MemberNotNull(nameof(RootPath), nameof(_path), nameof(_name))]
        init
        {
            _path = value;
            RootPath = IOPath.GetDirectoryName(value) ?? throw new InvalidOperationException($"{value}'s parent directory is null.");
            _name = IOPath.GetFileName(Path);
        }
    }

    private string _name;

    /// <inheritdoc />
    public override string Name => _name;

    /// <summary>
    /// Initializes a new instance of <see cref="FileTestProject"/>.
    /// </summary>
    public FileTestProject()
    {
    }

    /// <inheritdoc />
    public override Instance Clone(string instanceName, ITestOutputHelper logger)
    {
        return new(this, instanceName, logger);
    }
}
