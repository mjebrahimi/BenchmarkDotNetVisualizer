using System.Text.RegularExpressions;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Number Extensions
/// </summary>
public static partial class NumberExtensions
{
    /// <summary>
    /// Extracts the number from the <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public static decimal ExtractNumber(this string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input, nameof(input));

        var str = GetExtractNumberRegex().Match(input).Value;// .Value.Replace(",", "");
        return decimal.Parse(str);
    }

    /// <summary>
    /// Extracts the number from the <paramref name="input"/> or default.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public static decimal ExtractNumberOrDefault(this string input)
    {
        TryExtractNumber(input, out var value);
        return value;
    }

    /// <summary>
    /// Tries to extract the number from the <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool TryExtractNumber(this string input, out decimal value)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            value = default;
            return false;
        }

        var str = GetExtractNumberRegex().Match(input).Value;// .Value.Replace(",", "");
        return decimal.TryParse(str, out value);
    }

    /// <summary>
    /// Determines whether the input starts with a number.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    ///   <c>true</c> if the input starts with a number; otherwise, <c>false</c>.
    /// </returns>
    public static bool StartsWithNumber(this string input)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        return GetStartsWithNumberRegex().IsMatch(input);
    }

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"[\d,]+(?:\.\d+)?")]
    internal static partial Regex GetExtractNumberRegex();

    [GeneratedRegex(@"^\d")]
    internal static partial Regex GetStartsWithNumberRegex();
#else
    internal static Regex GetExtractNumberRegex() => ExtractNumberRegex;
    internal static readonly Regex ExtractNumberRegex = new(@"[\d,]+(?:\.\d+)?");
    internal static Regex GetStartsWithNumberRegex() => StartsWithNumberRegex;
    internal static readonly Regex StartsWithNumberRegex = new(@"^\d");
#endif
}
