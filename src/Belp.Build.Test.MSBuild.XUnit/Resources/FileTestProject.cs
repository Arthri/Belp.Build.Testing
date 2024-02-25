using Microsoft.Build.Evaluation;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using IOPath = System.IO.Path;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Represents a <see cref="TestProject"/> backed by a file.
/// </summary>
public class FileTestProject : TestProject
{
    /// <summary>
    /// Represents a test project instance for <see cref="FileTestProject"/>.
    /// </summary>
    /// <remarks>Instances must be created using <see cref="FileTestProject.Clone(string, ITestOutputHelper)"/>.</remarks>
    public class Instance : TestProjectInstance<FileTestProject>
    {
        /// <summary>
        /// Gets the clone's location.
        /// </summary>
        public string CacheLocation { get; }

        internal Instance(FileTestProject project, string instanceName, ITestOutputHelper logger)
            : base(project, instanceName, logger)
        {
            CacheLocation = IOPath.Combine(TestPaths.ProjectCache, instanceName);

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

            string sourceDirectory = Project.RootPath;

            foreach (string file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, IOPath.Combine(CacheLocation, IOPath.GetRelativePath(sourceDirectory, file)));
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

    private readonly Lazy<Project> _project;

    /// <inheritdoc />
    public override Project Project => _project.Value;

    private string _name;

    /// <inheritdoc />
    public override string Name => _name;

    /// <summary>
    /// Initializes a new instance of <see cref="FileTestProject"/>.
    /// </summary>
    public FileTestProject()
    {
        _project = new(() => Project.FromFile(_path, new()), true);
    }

    /// <inheritdoc />
    public override Instance Clone(string instanceName, ITestOutputHelper logger)
    {
        return new(this, instanceName, logger);
    }
}
