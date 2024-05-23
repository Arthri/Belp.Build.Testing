using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Collections;

namespace Belp.Build.Testing;

/// <summary>
/// Inherits from <see cref="BuildParameters"/> and sets additional default values.
/// </summary>
internal sealed class BuildParametersWithDefaults : BuildParameters
{
    /// <summary>
    /// Initializes a new instance of <see cref="BuildParameters"/> with additional default values.
    /// </summary>
    public BuildParametersWithDefaults(ILogger logger)
    {
        EnableNodeReuse = true;
        Loggers = Loggers == null
            ? new SingleEnumerable<ILogger>(logger)
            : Loggers.Append(logger)
            ;
    }
}

file sealed class SingleEnumerable<T> : IEnumerable<T>
{
    private sealed class Enumerator : IEnumerator<T>
    {
        private readonly T _element;
        private int _state;

        public T Current => _state switch
        {
            0 => throw new InvalidOperationException("Enumeration has not started. Call MoveNext."),
            1 => _element,
            2 => throw new InvalidOperationException("Enumeration already finished."),
            _ => throw new InvalidOperationException("Invalid state."),
        };

        object? IEnumerator.Current => Current;

        public Enumerator(T element)
        {
            _element = element;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_state is >= 0 and <= 1)
            {
                _state++;
                return _state == 1;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _state = 0;
        }
    }

    private readonly T _element;

    public SingleEnumerable(T element)
    {
        _element = element;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(_element);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
