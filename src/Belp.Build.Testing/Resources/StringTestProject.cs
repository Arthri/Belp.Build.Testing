using Microsoft.Build.Evaluation;
using IOPath = System.IO.Path;

namespace Belp.Build.Testing.Resources;

/// <summary>
/// Represents a <see cref="TestProject" /> loaded from XML in a string.
/// </summary>
public sealed class StringTestProject : TestProject
{
    /// <summary>
    /// Represents a test project instance for <see cref="StringTestProject" />.
    /// </summary>
    /// <remarks>Instances must be created using <see cref="StringTestProject.Clone()"/>.</remarks>
    private sealed class Instance : TestProjectInstance<StringTestProject>
    {
        /// <summary>
        /// Gets the directory containing this instance.
        /// </summary>
        private readonly string _directory;

        private readonly string _projectPath;

        private readonly Lazy<Project> _project;

        /// <inheritdoc />
        public override Project MSBuildProject => _project.Value;

        internal Instance(StringTestProject project)
            : base(project)
        {
            _directory = TestPaths.GetTempProjectDirectory();
            _projectPath = IOPath.Combine(_directory, TestProject.Name);

            _project = new(() => Project.FromFile(_projectPath, new()), true);

            if (!Directory.Exists(_directory))
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
                Directory.Delete(_directory, true);
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

            _ = Directory.CreateDirectory(_directory);
            File.WriteAllText(_projectPath, TestProject.Contents);
        }
    }

    /// <summary>
    /// Gets the project's contents in XML.
    /// </summary>
    public string Contents { get; }

    /// <inheritdoc />
    public override string Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="StringTestProject"/> with the specified <paramref name="contents"/>.
    /// </summary>
    /// <param name="fileName">The project's name including the file extension.</param>
    /// <param name="contents">The project's contents in XML.</param>
    public StringTestProject(string fileName, string contents)
    {
        Name = fileName;
        Contents = contents;
    }

    /// <inheritdoc />
    public override TestProjectInstance Clone()
    {
        return new Instance(this);
    }
}
