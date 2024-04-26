using Microsoft.Build.Evaluation;
using IOPath = System.IO.Path;

namespace Belp.Build.Test.MSBuild.Resources;

/// <summary>
/// Represents a <see cref="TestProject" /> loaded from XML in a string.
/// </summary>
public sealed class StringTestProject : TestProject
{
    /// <summary>
    /// Represents a test project instance for <see cref="StringTestProject" />.
    /// </summary>
    /// <remarks>Instances must be created using <see cref="StringTestProject.Clone()"/>.</remarks>
    public sealed class Instance : TestProjectInstance<StringTestProject>
    {
        /// <summary>
        /// Gets the clone's physical location.
        /// </summary>
        public string Location { get; }

        private readonly string _projectPath;

        private readonly Lazy<Project> _project;

        /// <inheritdoc />
        public override Project MSBuildProject => _project.Value;

        internal Instance(StringTestProject project)
            : base(project)
        {
            Location = IOPath.Combine(TestPaths.ProjectCache, Guid.NewGuid().ToString("N"));
            _projectPath = IOPath.Combine(Location, TestProject.Name);

            _project = new(() => Project.FromFile(_projectPath, new()), true);

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

            _ = Directory.CreateDirectory(Location);
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
    public override Instance Clone()
    {
        return new(this);
    }
}
