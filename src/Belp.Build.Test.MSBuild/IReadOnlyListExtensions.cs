using System.Collections;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Provides extensions for <see cref="IReadOnlyList{T}"/>.
/// </summary>
public static class IReadOnlyListExtensions
{
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
