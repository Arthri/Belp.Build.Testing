using Microsoft.Build.Execution;

namespace Belp.Build.Test.MSBuild.ObjectModel;

/// <summary>
/// Represents an item declared inside an MSBuild project.
/// </summary>
public readonly struct MSBuildItem
{
    /// <summary>
    /// Represents an item's metadata.
    /// </summary>
    public readonly struct ItemMetadata
    {
        private readonly ProjectMetadataInstance _metadata;

        /// <summary>
        /// Gets the metadata's name.
        /// </summary>
        public string Name => _metadata.Name;

        /// <summary>
        /// Gets the metadata's evaluated value.
        /// </summary>
        public string Value => _metadata.EvaluatedValue;

        internal ItemMetadata(ProjectMetadataInstance metadata)
        {
            _metadata = metadata;
        }
    }

    private readonly ProjectItemInstance _item;

    /// <summary>
    /// Gets the item's type.
    /// </summary>
    public string Type => _item.ItemType;

    /// <summary>
    /// Gets the item's evaluated include.
    /// </summary>
    public string Include => _item.EvaluatedInclude;

    /// <summary>
    /// Gets the item's metadata.
    /// </summary>
    public IEnumerable<ItemMetadata> Metadata
    {
        get
        {
            foreach (ProjectMetadataInstance? metadata in _item.Metadata)
            {
                yield return new ItemMetadata(metadata);
            }
        }
    }

    internal MSBuildItem(ProjectItemInstance item)
    {
        _item = item;
    }
}
