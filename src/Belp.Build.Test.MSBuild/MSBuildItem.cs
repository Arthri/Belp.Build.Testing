using Microsoft.Build.Execution;
using System.Collections.Frozen;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Represents an item declared inside an MSBuild project.
/// </summary>
public readonly struct MSBuildItem
{
    /// <summary>
    /// Gets the item's type.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the item's identity or the evaluated contents of its Include attribute.
    /// </summary>
    public string Identity { get; }

    /// <summary>
    /// Gets the item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MSBuildItem"/> for the specified <paramref name="type"/> and <paramref name="identity"/> with the specified <paramref name="metadata"/>.
    /// </summary>
    /// <param name="type">The item's type.</param>
    /// <param name="identity">The item's identity or the contents of the Include attribute</param>
    /// <param name="metadata">The item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata. Defaults to <see langword="null" /> or no metadata.</param>
    public MSBuildItem(string type, string identity, IReadOnlyDictionary<string, string>? metadata = null)
    {
        Type = type;
        Identity = identity;
        Metadata = metadata ?? FrozenDictionary<string, string>.Empty;
    }

    internal MSBuildItem(ProjectItemInstance item)
    {
        Type = item.ItemType;
        Identity = item.EvaluatedInclude;
        Metadata = item.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue).AsReadOnly();
    }
}
