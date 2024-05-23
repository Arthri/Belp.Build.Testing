using Microsoft.Build.Evaluation;
using System.Diagnostics.CodeAnalysis;
using IOPath = System.IO.Path;

namespace Belp.Build.Testing.Resources;

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
        /// Gets the directory containing this instance.
        /// </summary>
        public string Directory { get; }

        private readonly Lazy<Project> _project;

        /// <inheritdoc />
        public override Project MSBuildProject => _project.Value;

        internal Instance(FileTestProject project, string? directory = null)
            : base(project)
        {
            directory ??= TestPaths.GetTempProjectDirectory();
            Directory = directory;
            _project = new(() => Project.FromFile(IOPath.Combine(Directory, IOPath.GetRelativePath(TestProject.Directory, TestProject.Path)), new()), true);

            if (!System.IO.Directory.Exists(Directory))
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
                System.IO.Directory.Delete(Directory, true);
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

            string sourceDirectory = TestProject.Directory;

            foreach (string file in System.IO.Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string destinationPath = IOPath.Combine(Directory, IOPath.GetRelativePath(sourceDirectory, file));
                string? destinationDirectory = IOPath.GetDirectoryName(destinationPath);
                if (destinationDirectory is not null)
                {
                    _ = System.IO.Directory.CreateDirectory(destinationDirectory);
                }
                File.Copy(file, destinationPath);
            }
        }
    }

    /// <summary>
    /// Gets the directory containing this test project.
    /// </summary>
    public string Directory { get; private init; }

    private readonly string _path;

    /// <summary>
    /// Gets the path to the MSBuild project file.
    /// </summary>
    public required string Path
    {
        get => _path;

        [MemberNotNull(nameof(Directory), nameof(_path), nameof(_name))]
        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!IOPath.IsPathRooted(value))
            {
                throw new UnrootedProjectPathException(value);
            }

            _path = value;
            Directory = IOPath.GetDirectoryName(value) ?? throw new InvalidOperationException($"{value}'s parent directory is null.");
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

    internal Instance CloneInto(string directory)
    {
        return new(this, directory);
    }
}
