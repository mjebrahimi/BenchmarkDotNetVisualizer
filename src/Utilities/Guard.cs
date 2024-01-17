using System.ComponentModel;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Guard and Validation
/// </summary>
public static class Guard
{
    /// <summary>Throws an exception if <paramref name="argument"/> is null or empty.</summary>
    /// <param name="argument">The argument to validate as non-null and non-empty.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowIfNullOrEmpty<T>([System.Diagnostics.CodeAnalysis.NotNull] IEnumerable<T> argument,
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))]
#endif
        string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);

        ThrowIfEmpty(argument, paramName);
    }

    /// <summary>Throws an exception if <paramref name="argument"/> is empty.</summary>
    /// <param name="argument">The argument to validate as non-empty.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowIfEmpty<T>(IEnumerable<T> argument,
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))]
#endif
        string? paramName = null)
    {
        if (argument is null) return;

        //Performance Tip: using pattern matching on array is faster than TryGetNonEnumeratedCount
        if (argument is Array and { Length: 0 }
            || (argument.TryGetNonEnumeratedCount(out var count) && count == 0)
            || argument.Any() is false)
        {
            throw new System.ArgumentException("Argument is empty.", paramName);
        }
    }

#if NET8_0_OR_GREATER
    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not in the range.</summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="from">The starting value to compare with <paramref name="value"/>.</param>
    /// <param name="to">The ending value to compare with <paramref name="value"/>.</param>
    /// <param name="inclusive">if set to <c>true</c> inclusive.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void ThrowIfNotInRange<T>(T value, T from, T to, bool inclusive = true, [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (inclusive)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, from, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, to, paramName);
        }
        else
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, from, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, to, paramName);
        }
    }
#else
    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not in the range.</summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="from">The starting value to compare with <paramref name="value"/>.</param>
    /// <param name="to">The ending value to compare with <paramref name="value"/>.</param>
    /// <param name="inclusive">if set to <c>true</c> inclusive.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void ThrowIfNotInRange(double value, double from, double to, bool inclusive = true, string? paramName = null)
    {
        if (inclusive)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, from, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, to, paramName);
        }
        else
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, from, paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, to, paramName);
        }
    }
#endif

    /// <summary>Throws an <see cref="InvalidEnumArgumentException"/> if <paramref name="value"/> is not an enum defined value.</summary>
    /// <param name="value">The argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static void ThrowIfEnumNotDefined<TEnum>(TEnum value,
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))]
#endif
        string? paramName = null) where TEnum : struct, Enum
    {
        if (Enum.IsDefined(typeof(TEnum), value) is false)
            throw new InvalidEnumArgumentException(paramName, Convert.ToInt32(value), typeof(TEnum));
    }
}