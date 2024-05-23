namespace Belp.Build.Testing.Resources;

/// <summary>
/// Occurs when an expected XML element is not found inside a NuGet package specification(.nuspec) file.
/// </summary>
public class NuspecElementNotFoundException : NuspecException
{
    /// <summary>
    /// Gets an XPath to the expected location of the missing element.
    /// </summary>
    public string ElementPath { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="NuspecElementNotFoundException"/> for the specified <paramref name="path"/> with the specified <paramref name="elementPath"/>.
    /// </summary>
    /// <param name="path"><inheritdoc cref="NuspecException(string, string?)" path="/param[@name='path']" /></param>
    /// <param name="elementPath">An XPath to the expected location of the missing element.</param>
    public NuspecElementNotFoundException(string path, string elementPath)
        : base(path, $"{elementPath} was not found in the .nuspec file located at {path}.")
    {
        ElementPath = elementPath;
    }
}
