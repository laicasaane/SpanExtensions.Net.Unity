﻿namespace SpanExtensions;

/// <summary>
/// Allows for use in a foreach construct
/// </summary>
/// <typeparam name="T">The type of elements in the enumerated <see cref="ReadOnlySpan{T}"/></typeparam>
public ref struct SpanSplitSequenceWithCountEnumerator<T> where T : IEquatable<T>
{
    ReadOnlySpan<T> Span;
    readonly ReadOnlySpan<T> Delimiter;
    readonly int Count;
    int currentCount;

    public ReadOnlySpan<T> Current { get; internal set; }

    public SpanSplitSequenceWithCountEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter, int count)
    {
        Span = span;
        Delimiter = delimiter;
        Count = count;
    }

    public SpanSplitSequenceWithCountEnumerator<T> GetEnumerator()
    {
        return this;
    }

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns><code>true</code> if the enumerator was successfully advanced to the next element; <code>false</code> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
        var span = Span;
        if (span.IsEmpty)
        {
            return false;
        }
        if (currentCount == Count)
        {
            return false;
        }
        int index = span.IndexOf(Delimiter);
        if (index == -1 || index >= span.Length)
        {
            Span = ReadOnlySpan<T>.Empty;
            Current = span;
            return true;
        }
        currentCount++;
        Current = span[..index];
        Span = span[(index + Delimiter.Length)..];
        return true;
    }

}