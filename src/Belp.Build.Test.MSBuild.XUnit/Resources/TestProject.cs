namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Provides an interface for projects inside test samples.
/// </summary>
public sealed class TestProject
{
    /// <summary>
    /// Gets the path the project is located in.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string RootPath { get; private init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private readonly string _path;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the path to the project file.
    /// </summary>
    public required string Path
    {
        get => _path;

        init
        {
            _path = value;
            RootPath = System.IO.Path.GetDirectoryName(value) ?? throw new InvalidOperationException($"{value}'s parent directory is null.");
        }
    }
}
