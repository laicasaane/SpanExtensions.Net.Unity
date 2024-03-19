﻿using System;

namespace SpanExtensions
{
    public static partial class ReadOnlySpanExtensions
    {
        /// <summary>
        /// Returns the only element in <see cref="ReadOnlySpan{T}"/>, and throws an exception if there is not exactly one element in the <see cref="ReadOnlySpan{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to return the single element of.</param>
        /// <returns>The single element in <paramref name="source"/>.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> contains more than one element -or- <paramref name="source"/> is empty.</exception>
        public static T Single<T>(this ReadOnlySpan<T> source)
        {
            if(source.Length != 1)
            {
                throw new InvalidOperationException($"{nameof(source)} must contain only one element.");
            }
            return source[0];
        }

        /// <summary>
        /// Returns the only element in <see cref="ReadOnlySpan{T}"/> that satisfies a specified condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <typeparam name="T">The type of elements in <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="ReadOnlySpan{T}"/> to return the single element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The single element in <paramref name="source"/> that satisfies a condition.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null.</exception> 
        /// <exception cref="InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>. -or- More than one element satisfies the condition in <paramref name="predicate"/>. -or- <paramref name="source"/> is empty.</exception>
        public static T Single<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
        {
            if(predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            T single = default!;
            bool hassingle = false;
            for(int i = 0; i < source.Length; i++)
            {
                T item = source[i];

                if(predicate(item))
                {
                    if(hassingle)
                    {
                        throw new InvalidOperationException($"{nameof(source)} must contain only one element that matches {nameof(predicate)}.");
                    }
                    single = item;
                    hassingle |= true;
                }
            }

            if(!hassingle)
            {
                throw new InvalidOperationException("No element matched the specified condition.");
            }
            return single;
        }
    }
}