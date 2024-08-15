using System.Dynamic;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Enumerable Extensions
/// </summary>
public static partial class EnumerableExtensions
{
#pragma warning disable S4136 // Method overloads should be grouped together
    #region Ascending
    /// <summary>
    /// Orders the by properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByProperties(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByProperties(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    /// <summary>
    /// Orders the by properties automatically.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByPropertiesAuto(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByPropertiesAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }

    /// <summary>
    /// Orders the by numeric properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByNumericProperties(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByNumericProperties(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Ascending));
    }
    #endregion

    #region Descending
    /// <summary>
    /// Orders the by properties descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByPropertiesDescending(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByProperties(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    /// <summary>
    /// Orders the by properties automatically descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByPropertiesAutoDescending(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByPropertiesAuto(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }

    /// <summary>
    /// Orders the by numeric properties descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByNumericPropertiesDescending(this IEnumerable<ExpandoObject> enumerable, params string[] orderProperties)
    {
        return OrderByNumericProperties(enumerable, orderProperties.ToDictionary(p => p, _ => OrderMode.Descending));
    }
    #endregion

    #region IDictionary
    /// <summary>
    /// Orders the by properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByProperties(this IEnumerable<ExpandoObject> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<ExpandoObject> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(expando => expando.GetPropertyOrDefault(orderProperty.Key)) :
                        orderdItems!.ThenBy(expando => expando.GetPropertyOrDefault(orderProperty.Key));
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key)) :
                        orderdItems!.ThenByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key));
                    break;
            }
        }
        return orderdItems;
    }

    /// <summary>
    /// Orders the by properties automatically.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByPropertiesAuto(this IEnumerable<ExpandoObject> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<ExpandoObject> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.GetDecimalIfIsNumericElseString()) :
                        orderdItems!.ThenBy(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.GetDecimalIfIsNumericElseString());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.GetDecimalIfIsNumericElseString()) :
                        orderdItems!.OrderByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.GetDecimalIfIsNumericElseString());
                    break;
            }
        }
        return orderdItems;
    }

    /// <summary>
    /// Orders the by numeric properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderProperties">The order properties.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<ExpandoObject> OrderByNumericProperties(this IEnumerable<ExpandoObject> enumerable, IDictionary<string, OrderMode> orderProperties)
    {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderProperties, nameof(orderProperties));

        IOrderedEnumerable<ExpandoObject> orderdItems = null!;
        for (int j = 0; j < orderProperties.Count; j++)
        {
            var orderProperty = orderProperties.ElementAt(j);

            switch (orderProperty.Value)
            {
                case OrderMode.Ascending:
                    orderdItems = j == 0 ?
                        enumerable.OrderBy(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenBy(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.ToString()?.ExtractNumber());
                    break;
                case OrderMode.Descending:
                    orderdItems = j == 0 ?
                        enumerable.OrderByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.ToString()?.ExtractNumber()) :
                        orderdItems!.ThenByDescending(expando => expando.GetPropertyOrDefault(orderProperty.Key)?.ToString()?.ExtractNumber());
                    break;
            }
        }
        return orderdItems;
    }
    #endregion

    /// <summary>
    /// Gets the decimal value if the value is numeric else gets string of the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    internal static object? GetDecimalIfIsNumericElseString(this object? value)
    {
        if (value is null)
            return null;

        var str = value.ToString()?.RemoveMarkdownBold();
        if (str == "-") return 0M; //for Allocated column with zero-allocation value
        return str?.StartsWithNumber() is true ? str.ExtractNumber() : str;
    }
#pragma warning restore S4136 // Method overloads should be grouped together
}

/// <summary>
/// Order Mode
/// </summary>
public enum OrderMode
{
    /// <summary>
    /// Ascending order mode
    /// </summary>
    Ascending,

    /// <summary>
    /// Descending order mode
    /// </summary>
    Descending,
}