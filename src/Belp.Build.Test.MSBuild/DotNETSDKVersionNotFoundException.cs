namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Occurs when the specified minimum version of the .NET SDK was not found.
/// </summary>
public class DotNETSDKVersionNotFoundException : Exception
{
    /// <summary>
    /// Gets the minimum version of the .NET SDK that was searched for.
    /// </summary>
    public int MinimumVersion { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DotNETSDKVersionNotFoundException"/> with the specified <paramref name="minimumVersion"/>.
    /// </summary>
    /// <param name="minimumVersion">The minimum version of the .NET SDK that was searched for.</param>
    public DotNETSDKVersionNotFoundException(int minimumVersion) : base($"Unable to find .NET SDK version {minimumVersion} or higher.")
    {
        MinimumVersion = minimumVersion;
    }
}
