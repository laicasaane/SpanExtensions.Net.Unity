﻿namespace SpanExtensions;
 
/// <summary>
/// Allows for use in a foreach construct
/// </summary>
/// <typeparam name="T">The type of elements in the enumerated <see cref="ReadOnlySpan{T}"/></typeparam>
public ref struct SpanSplitAnyWithCountEnumerator<T> where T : IEquatable<T>
{
    ReadOnlySpan<T> Span;
    readonly ReadOnlySpan<T> Delimiters;
    readonly int Count;
    int currentCount;

    public ReadOnlySpan<T> Current { get; internal set; }

    public SpanSplitAnyWithCountEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters, int count)
    {
        Span = span;
        Delimiters = delimiters;
        Count = count;

    }
    public SpanSplitAnyWithCountEnumerator<T> GetEnumerator()
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
        int index = span.IndexOfAny(Delimiters);
        if (index == -1 || index >= span.Length)
        {
            Span = ReadOnlySpan<T>.Empty;
            Current = span;
            return true;
        }
        currentCount++;
        Current = span[..index];
        Span = span.Slice(index + 1);
        return true;
    }

}