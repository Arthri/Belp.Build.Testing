using Microsoft.Build.Execution;
using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Belp.Build.Test.MSBuild;

/// <summary>
/// Represents an item declared inside an MSBuild project.
/// </summary>
public readonly record struct MSBuildItem : IEquatable<MSBuildItem.Untyped>
{
    private sealed class EquatableDictionary : IReadOnlyDictionary<string, string>, IEquatable<IReadOnlyDictionary<string, string>>
    {
        private readonly IReadOnlyDictionary<string, string> _dictionary;

        public string this[string key] => _dictionary[key];

        public IEnumerable<string> Keys => _dictionary.Keys;

        public IEnumerable<string> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        internal EquatableDictionary(IReadOnlyDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override bool Equals(object? obj)
        {
            return obj is IReadOnlyDictionary<string, string> dictionary && Equals(dictionary);
        }

        public bool Equals(IReadOnlyDictionary<string, string>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> kvp in this)
            {
                if (!other.TryGetValue(kvp.Key, out string? otherValue))
                {
                    return false;
                }

                if (kvp.Value != otherValue)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (KeyValuePair<string, string> kvp in this.OrderBy(kvp => kvp.Key, StringComparer.InvariantCulture))
            {
                hashCode.Add(kvp.Key);
                hashCode.Add(kvp.Value);
            }

            return hashCode.ToHashCode();
        }
    }

    /// <summary>
    /// Represents an MSBuild item without a type.
    /// </summary>
    public readonly record struct Untyped
    {
        /// <summary>
        /// Gets the item's identity or the evaluated contents of its Include attribute.
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// Gets the item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; }

        internal Untyped(string identity, IReadOnlyDictionary<string, string> metadata)
        {
            Identity = identity;
            Metadata = metadata;
        }

        internal Untyped(MSBuildItem item)
        {
            Identity = item.Identity;
            Metadata = item.Metadata;
        }
    }

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

    internal MSBuildItem(string type, string identity, IReadOnlyDictionary<string, string> metadata)
    {
        Type = type;
        Identity = identity;
        Metadata = metadata;
    }

    internal MSBuildItem(ProjectItemInstance item)
    {
        Type = item.ItemType;
        Identity = item.EvaluatedInclude;
        Metadata = new EquatableDictionary(item.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue));
    }

    /// <inheritdoc />
    public bool Equals(Untyped other)
    {
        return Identity == other.Identity && Metadata.Equals(other.Metadata);
    }

    /// <summary>
    /// Creates a new instance of <see cref="MSBuildItem"/> for the specified <paramref name="type"/> and <paramref name="identity"/> with the specified <paramref name="metadata"/>.
    /// </summary>
    /// <param name="type">The item's type.</param>
    /// <param name="identity">The item's identity or the contents of the Include attribute</param>
    /// <param name="metadata">The item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata. Defaults to <see langword="null" /> or no metadata.</param>
    /// <returns>The new instance of <see cref="MSBuildItem"/> for the specified <paramref name="type"/> and <paramref name="identity"/> with the specified <paramref name="metadata"/>.</returns>
    public static MSBuildItem Create(string type, string identity, IReadOnlyDictionary<string, string>? metadata = null)
    {
        return new(type, identity, new EquatableDictionary(metadata ?? FrozenDictionary<string, string>.Empty));
    }

    /// <summary>
    /// Creates a new untyped instance of <see cref="MSBuildItem"/> for the specified <paramref name="identity"/> with the specified <paramref name="metadata"/>.
    /// </summary>
    /// <param name="identity">The item's identity or the contents of the Include attribute</param>
    /// <param name="metadata">The item's metadata in dictionary form. Dictionary keys represent metadata names, while values associated with those keys are the values associated with the metadata. Defaults to <see langword="null" /> or no metadata.</param>
    /// <returns>The new untyped instance of <see cref="MSBuildItem"/> for the specified <paramref name="identity"/> with the specified <paramref name="metadata"/>.</returns>
    public static Untyped Create(string identity, IReadOnlyDictionary<string, string>? metadata = null)
    {
        return new(identity, new EquatableDictionary(metadata ?? FrozenDictionary<string, string>.Empty));
    }
}
