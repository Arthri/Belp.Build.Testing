using Microsoft.Build.Execution;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Belp.Build.Test.MSBuild.ObjectModel;

/// <summary>
/// A facade which encapsulates the output of MSBuild to provide a more consistent and usable API.
/// </summary>
public sealed class MSBuildResult
{
    private readonly BuildResult _buildResult;

    private sealed class ConcatenatedDiagnosticsList(IReadOnlyList<Diagnostic> warnings, IReadOnlyList<Diagnostic> errors) : IReadOnlyList<Diagnostic>
    {
        /// <inheritdoc />
        public Diagnostic this[int index] => index < errors.Count ? errors[index] : warnings[index - errors.Count];

        /// <inheritdoc />
        public int Count => warnings.Count + errors.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return errors.Concat(warnings).GetEnumerator();
        }
    }

    /// <summary>
    /// Gets a list of captured warnings from the build process.
    /// </summary>
    public IReadOnlyList<Diagnostic> Warnings { get; }

    /// <summary>
    /// Gets a list of captured errors from the build process.
    /// </summary>
    public IReadOnlyList<Diagnostic> Errors { get; }

    /// <summary>
    /// Gets a list of captured warnings and erros from the build process.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics => new ConcatenatedDiagnosticsList(Warnings, Errors);

    /// <inheritdoc cref="BuildResult.OverallResult" />
    public BuildResultCode OverallResult => _buildResult.OverallResult;

    /// <summary>
    /// Gets a collection of properties extracted from the build result.
    /// </summary>
    public IReadOnlyDictionary<string, string?> Properties { get; }

    /// <summary>
    /// Gets a collection of items extracted from the build result.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<MSBuildItem>> Items { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MSBuildResult"/> with the specified <paramref name="logger"/>, <paramref name="buildResult"/> and <paramref name="projectStateAfterBuild"/>.
    /// </summary>
    /// <param name="logger">The logger used to capture the build output.</param>
    /// <param name="buildResult">The result of the build.</param>
    /// <param name="projectStateAfterBuild">The state of the project after the build, if present.</param>
    public MSBuildResult(XUnitMSBuildLoggerAdapter logger, BuildResult buildResult, ProjectInstance? projectStateAfterBuild)
    {
        Warnings = OrderDiagnostics(logger.Warnings);
        Errors = OrderDiagnostics(logger.Errors);
        _buildResult = buildResult;

        Properties = GetPropertiesFromProjectState(projectStateAfterBuild);
        Items = GetItemsFromProjectState(projectStateAfterBuild);
    }

    private static IReadOnlyDictionary<string, string?> GetPropertiesFromProjectState(ProjectInstance? projectStateAfterBuild)
    {
        if (projectStateAfterBuild is not null)
        {
            var properties = new Dictionary<string, string?>();

            foreach (ProjectPropertyInstance? property in projectStateAfterBuild.Properties)
            {
                properties[property.Name] = property.EvaluatedValue;
            }
            foreach ((string name, string value) in projectStateAfterBuild.GlobalProperties)
            {
                properties[name] = value;
            }

            if (properties.Count != 0)
            {
                return properties.AsReadOnly();
            }
        }

        return FrozenDictionary<string, string?>.Empty;
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<MSBuildItem>> GetItemsFromProjectState(ProjectInstance? projectStateAfterBuild)
    {
        if (projectStateAfterBuild is not null)
        {
            var items = new Dictionary<string, List<MSBuildItem>>();

            foreach (ProjectItemInstance? item in projectStateAfterBuild.Items)
            {
                string itemType = item.ItemType;
                if (!items.TryGetValue(itemType, out List<MSBuildItem>? itemGroup))
                {
                    items[itemType] = itemGroup = [];
                }

                itemGroup.Add(new MSBuildItem(item));
            }

            if (items.Count != 0)
            {
                return items
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => (IReadOnlyList<MSBuildItem>)kvp.Value.AsReadOnly()
                    )
                    .AsReadOnly()
                    ;
            }
        }

        return FrozenDictionary<string, IReadOnlyList<MSBuildItem>>.Empty;
    }

    private static ReadOnlyCollection<Diagnostic> OrderDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics
            .OrderBy(k => k.Severity)
            .ThenBy(k => k.Project)
            .ThenBy(k => k.Code)
            .ThenBy(k => k.Message)
            .ThenBy(k => k.File)
            .ThenBy(k => k.Span)
            .ToList()
            .AsReadOnly()
            ;
    }
}
