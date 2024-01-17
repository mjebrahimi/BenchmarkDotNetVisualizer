using System.Diagnostics.CodeAnalysis;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Enumerable Extensions
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Determines whether the specified enumerable is not null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    ///   <c>true</c> the specified enumerable is not null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return !IsNullOrEmpty(source);
    }

    /// <summary>
    /// Determines whether the specified source is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    ///   <c>true</c> if the specified source is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        if (source is null)
            return true;

        return source is Array { Length: 0 }
            || (source.TryGetNonEnumeratedCount(out var count) && count == 0)
            || source.Any() is false;
    }

    /// <summary>
    /// Splits/Chunks by value when occurrence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> SplitByOccurrence<T>(this IEnumerable<T> source, T value)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        //for example calling new[] { 0, 0, 1, 2, 0, 0, 3, 4, 0, 0 }.SplitByOccurrence(0).Select(p => p.ToList()).ToList() => must returns [ [1,2], [3,4] ]
        var section = 0;
        return source.GroupBy(p =>
        {
            if (p?.Equals(value) is not false)
                section++;
            return section;
        })
        .Select(p => p.Where(p => p?.Equals(value) is false))
        .Where(p => p.Any());
    }

    /// <summary>
    /// Splits/Chunks by condition when occurrence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> SplitByOccurrence<T>(this IEnumerable<T> source, Func<T, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));

        //for example calling new[] { 0, 0, 1, 2, 0, 0, 3, 4, 0, 0 }.SplitByOccurrence(p => p == 0).Select(p => p.ToList()).ToList() => must returns [ [1,2], [3,4] ]
        var section = 0;
        return source.GroupBy(p =>
        {
            if (condition(p))
                section++;
            return section;
        })
        .Select(p => p.Where(p => condition(p) is false))
        .Where(p => p.Any());
    }

    /// <summary>
    /// Splits/Chunks by key selector when occurrence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> SplitByOccurrence<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));

        //for example calling new[] { 0, 0, 1, 2, 0, 0, 3, 3, 0, 0 }.SplitByOccurrence(p => p == 0).Select(p => p.ToList()).ToList() => must returns [ [0, 0], [1], [2], [0, 0], [3, 3], [0, 0] ]
        var section = 0;
        TKey? previeusGroupValue = default;
        return source.GroupBy(p =>
        {
            var currentGroupValue = keySelector(p);
            var shouldSplit = previeusGroupValue is not null && currentGroupValue!.Equals(previeusGroupValue) is false;
            previeusGroupValue = currentGroupValue;

            if (shouldSplit)
                section++;
            return section;
        }).Select(p => p.Select(p => p));
    }

    /// <summary>
    /// Splits the by group by.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> SplitByGroup<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));

        //for example calling new[] { 0, 0, 1, 2, 0, 0, 3, 3, 0, 0 }.SplitByGroup(p => p == 0).Select(p => p.ToList()).ToList() => must returns [ [0, 0, 0, 0, 0, 0], [1], [2], [3, 3], ]
        return source
            .GroupBy(keySelector)
            .Select(p => p.AsEnumerable());
    }

    /// <summary>
    /// Concatenates all collections together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static IEnumerable<T> Concat<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.Aggregate((current, next) => current.Concat(next));
    }

    /// <summary>
    /// Concatenates all collections together by the item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public static IEnumerable<T?> ConcatBy<T>(this IEnumerable<IEnumerable<T>> source, T? item)
    {
        return source.Aggregate((current, next) => current.Concat([item]).Concat(next));
    }
}