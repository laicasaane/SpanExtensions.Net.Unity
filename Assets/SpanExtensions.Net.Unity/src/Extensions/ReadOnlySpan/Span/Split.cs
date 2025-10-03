using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !NET9_0_OR_GREATER

namespace SpanExtensions
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static partial class MemoryExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        static readonly char[] WhiteSpaceDelimiters = new char[] { ' ', '\t', '\n', '\v', '\f', '\r', '\u0085', '\u00A0', '\u1680',
                        '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009',
                        '\u200A', '\u2028', '\u2029', '\u202F', '\u205F', '\u3000' };
#if NET8_0
        static readonly SearchValues<char> WhiteSpaceSearchValues = SearchValues.Create(WhiteSpaceDelimiters);
#endif

        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using the provided separator character.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The source span to be enumerated.</param>
        /// <param name="separator">The separator character to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, T separator) where T : IEquatable<T>
        {
            return new SpanSplitEnumerator<T>(source, separator);
        }

        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using the provided separator span.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The source span to be enumerated.</param>
        /// <param name="separator">The separator span to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T>
        {
            return new SpanSplitEnumerator<T>(source, separator, SpanSplitEnumeratorMode.Sequence);
        }

        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using any of the provided elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The source span to be enumerated.</param>
        /// <param name="separators">The separators to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
        public static SpanSplitEnumerator<T> SplitAny<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separators) where T : IEquatable<T>
        {
            if(separators.Length == 0 && typeof(T) == typeof(char))
            {
#if NET8_0
                return new SpanSplitEnumerator<T>(source, Unsafe.As<SearchValues<T>>(WhiteSpaceSearchValues));
#elif NET5_0_OR_GREATER
                ref char data = ref MemoryMarshal.GetArrayDataReference(WhiteSpaceDelimiters);
                ref T convertedData = ref Unsafe.As<char, T>(ref data);
                separators = MemoryMarshal.CreateReadOnlySpan(ref convertedData, WhiteSpaceDelimiters.Length);
#else
                unsafe
                {
                    fixed(char* ptr = &WhiteSpaceDelimiters[0])
                    {
                        separators = new ReadOnlySpan<T>(ptr, WhiteSpaceDelimiters.Length);
                    }
                }
#endif
            }

            return new SpanSplitEnumerator<T>(source, separators, SpanSplitEnumeratorMode.Any);
        }

#if NET8_0
        /// <summary>
        /// Returns a type that allows for enumeration of each element within a split span
        /// using the provided <see cref="SpanSplitEnumerator{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">The source span to be enumerated.</param>
        /// <param name="separators">The <see cref="SpanSplitEnumerator{T}"/> to be used to split the provided span.</param>
        /// <returns>Returns a <see cref="SpanSplitEnumerator{T}"/>.</returns>
        /// <remarks>
        /// Unlike <see cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>, the <paramref name="separators"/> is not checked for being empty.
        /// An empty <paramref name="separators"/> will result in no separators being found, regardless of the type of <typeparamref name="T"/>, whereas <see cref="SplitAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/> will use all Unicode whitespace characters as separators if <paramref name="separators"/> is empty and <typeparamref name="T"/> is <see cref="char"/>.
        /// </remarks>
        public static SpanSplitEnumerator<T> SplitAny<T>(this ReadOnlySpan<T> source, SearchValues<T> separators) where T : IEquatable<T>
        {
            return new SpanSplitEnumerator<T>(source, separators);
        }
#endif

    }
}

#endif