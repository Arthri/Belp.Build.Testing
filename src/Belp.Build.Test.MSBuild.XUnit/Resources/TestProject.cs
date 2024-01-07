using System.Diagnostics.CodeAnalysis;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Provides an interface for projects inside test samples.
/// </summary>
public sealed class TestProject
{
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

        [MemberNotNull(nameof(RootPath), nameof(_path))]
        init
        {
            _path = value;
            RootPath = System.IO.Path.GetDirectoryName(value) ?? throw new InvalidOperationException($"{value}'s parent directory is null.");
        }
    }
}
