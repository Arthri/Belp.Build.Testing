using Microsoft.Build.Evaluation;
using System.Diagnostics.CodeAnalysis;
using IOPath = System.IO.Path;

namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Represents a <see cref="TestProject"/> backed by a file.
/// </summary>
public sealed class FileTestProject : TestProject
{
    /// <summary>
    /// Represents a test project instance for <see cref="FileTestProject"/>.
    /// </summary>
    /// <remarks>Instances must be created using <see cref="FileTestProject.Clone()"/>.</remarks>
    public sealed class Instance : TestProjectInstance<FileTestProject>
    {
        /// <summary>
        /// Gets the clone's physical location.
        /// </summary>
        public string Location { get; }

        private readonly Lazy<Project> _project;

        /// <inheritdoc />
        public override Project MSBuildProject => _project.Value;

        internal Instance(FileTestProject project)
            : base(project)
        {
            Location = IOPath.Combine(TestPaths.ProjectCache, Guid.NewGuid().ToString("N"));
            _project = new(() => Project.FromFile(IOPath.Combine(Location, IOPath.GetRelativePath(TestProject.RootPath, TestProject.Path)), new()), true);

            if (!Directory.Exists(Location))
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
                Directory.Delete(Location, true);
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
                string destinationPath = IOPath.Combine(Location, IOPath.GetRelativePath(sourceDirectory, file));
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
    public override Instance Clone()
    {
        return new(this);
    }
}
