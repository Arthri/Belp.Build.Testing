namespace Belp.Build.Testing.Resources;

/// <summary>
/// Occurs when a NuGet package specification(.nuspec) file does not have a root element.
/// </summary>
public class NuspecRootNotFoundException : NuspecException
{
    /// <summary>
    /// Initializes a new instance of <see cref="NuspecRootNotFoundException"/> for the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path"><inheritdoc cref="NuspecException(string, string?)" path="/param[@name='path']" /></param>
    public NuspecRootNotFoundException(string path)
        : base(path, $"The .nuspec file located at {path} does not contain a root element.")
    {
    }
}
