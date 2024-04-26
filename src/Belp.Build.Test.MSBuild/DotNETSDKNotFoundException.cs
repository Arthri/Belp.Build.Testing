namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Occurs when the .NET SDK is not found by MSBuildLocator.
/// </summary>
public class DotNETSDKNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="DotNETSDKNotFoundException" />.
    /// </summary>
    public DotNETSDKNotFoundException() : base("Unable to find the .NET SDK.")
    {
    }
}
