namespace Belp.Build.Testing.Resources;

/// <summary>
/// Provides extensions for <see cref="TestProjectInstance"/>.
/// </summary>
public static class TestProjectInstanceExtensions
{
    /// <summary>
    /// Restores the project instance.
    /// </summary>
    /// <typeparam name="T">The type of the project instance.</typeparam>
    /// <param name="projectInstance">The project instance to restore.</param>
    /// <returns>The provided <paramref name="projectInstance"/>.</returns>
    public static T Restore<T>(this T projectInstance)
        where T : TestProjectInstance
    {
        projectInstance.Restore();
        return projectInstance;
    }
}
