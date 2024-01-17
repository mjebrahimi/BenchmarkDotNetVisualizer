using System.Diagnostics.CodeAnalysis;

#pragma warning disable S2166 // Classes named like "Exception" should extend "Exception" or a subclass

#if !NET8_0_OR_GREATER
/// <summary>
/// ArgumentException
/// </summary>
internal static class ArgumentException
{
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? str, string? paramName)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new System.ArgumentException($"Parameter '{paramName}' is null or white-space.", paramName);
    }

    public static void ThrowIfNullOrEmpty([NotNull] string? str, string? paramName)
    {
        if (string.IsNullOrEmpty(str))
            throw new System.ArgumentException($"Parameter '{paramName}' is null or empty.", paramName);
    }
}
#endif

#if !NET6_0_OR_GREATER
/// <summary>
/// ArgumentNullException
/// </summary>
internal static class ArgumentNullException
{
    public static void ThrowIfNull([NotNull] object? obj, string? paramName)
    {
#pragma warning disable RCS1256 // Invalid argument null check
        if (obj is null)
            throw new System.ArgumentNullException(paramName, $"Parameter '{paramName}' is null.");
#pragma warning restore RCS1256 // Invalid argument null check
    }
}
#endif

#if !NET8_0_OR_GREATER
/// <summary>
/// ArgumentOutOfRangeException
/// </summary>
internal static class ArgumentOutOfRangeException
{
    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
            ThrowLess(value, other, paramName);
    }

    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfGreaterThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
            ThrowGreater(value, other, paramName);
    }

    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as greater than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfLessThanOrEqual<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) <= 0)
            ThrowLessEqual(value, other, paramName);
    }

    /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal <paramref name="other"/>.</summary>
    /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
    /// <param name="other">The value to compare with <paramref name="value"/>.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
    public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
            ThrowGreaterEqual(value, other, paramName);
    }

    private static void ThrowLess<T>(T value, T other, string? paramName) =>
        throw new System.ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' (value: {value}) must be greater than or equal to {other}");

    private static void ThrowGreater<T>(T value, T other, string? paramName) =>
        throw new System.ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' (value: {value}) must be less than or equal to {other}");

    private static void ThrowLessEqual<T>(T value, T other, string? paramName) =>
        throw new System.ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' (value: {value}) must be greater than {other}");

    private static void ThrowGreaterEqual<T>(T value, T other, string? paramName) =>
        throw new System.ArgumentOutOfRangeException(paramName, value, $"Parameter '{paramName}' (value: {value}) must be less than {other}");
}
#endif
#pragma warning restore S2166 // Classes named like "Exception" should extend "Exception" or a subclass