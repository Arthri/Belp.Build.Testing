using System.Collections;
using System.Collections.Frozen;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Provides extensions for <see cref="IReadOnlyList{T}"/>.
/// </summary>
public static class IReadOnlyListExtensions
{
    /// <summary>
    /// Represents an MSBuild item without a type.
    /// </summary>
    public readonly record struct MSBuildItemWithoutType
    {
        /// <summary>
        /// Gets the item's identity or the evaluated contents of its Include attribute.
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// Gets the item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; }

        internal MSBuildItemWithoutType(string identity, IReadOnlyDictionary<string, string> metadata)
        {
            Identity = identity;
            Metadata = metadata;
        }

        internal MSBuildItemWithoutType(MSBuildItem item)
        {
            Identity = item.Identity;
            Metadata = item.Metadata;
        }

        /// <summary>
        /// Defines an implicit conversion from an item identity to a metadata-less <see cref="MSBuildItemWithoutType" />.
        /// </summary>
        /// <param name="identity">The identity of the item, or the contents of the Include attribute.</param>
        /// <returns>The converted metadata-less item.</returns>
        public static implicit operator MSBuildItemWithoutType(string identity)
        {
            return new(identity, FrozenDictionary<string, string>.Empty);
        }

        /// <summary>
        /// Defines an implicit conversion from a tuple-form item to a <see cref="MSBuildItemWithoutType" />.
        /// </summary>
        /// <param name="item">The tuple-form item to convert.</param>
        /// <returns>The converted tuple-form item.</returns>
        public static implicit operator MSBuildItemWithoutType((string Identity, (string Name, string Value)[] Metadata) item)
        {
            return new(item.Identity, item.Metadata.ToDictionary(m => m.Name, m => m.Value));
        }
    }

    private sealed class UntypedMSBuildItemList : IReadOnlyList<MSBuildItemWithoutType>
    {
        private readonly IReadOnlyList<MSBuildItem> _items;

        public MSBuildItemWithoutType this[int index] => new(_items[index]);

        public int Count => _items.Count;

        internal UntypedMSBuildItemList(IReadOnlyList<MSBuildItem> items)
        {
            _items = items;
        }

        public IEnumerator<MSBuildItemWithoutType> GetEnumerator()
        {
            foreach (MSBuildItem item in _items)
            {
                yield return new MSBuildItemWithoutType(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Removes the item type from the specified items.
    /// </summary>
    /// <param name="items">The list of items to remove the types from.</param>
    /// <returns>A list of items without attached item types.</returns>
    public static IReadOnlyList<MSBuildItemWithoutType> RemoveTypes(this IReadOnlyList<MSBuildItem> items)
    {
        return new UntypedMSBuildItemList(items);
    }
}
