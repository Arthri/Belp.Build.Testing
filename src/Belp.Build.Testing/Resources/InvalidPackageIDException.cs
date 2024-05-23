namespace Belp.Build.Testing.Resources;

/// <summary>
/// Occurs when the ID of the NuGet package is invalid.
/// </summary>
public class InvalidPackageIDException : Exception
{
    /// <summary>
    /// Gets the path to the package in question.
    /// </summary>
    public string PackagePath { get; }

    /// <summary>
    /// Gets the invalid ID in question.
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="InvalidPackageIDException"/> for the specified <paramref name="packagePath" /> and with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="packagePath">The path to the package in question.</param>
    /// <param name="id">The invalid ID in question.</param>
    public InvalidPackageIDException(string packagePath, string id)
        : base($"""The package located at {packagePath} has the invalid ID "{id}".""")
    {
        PackagePath = packagePath;
        ID = id;
    }
}
