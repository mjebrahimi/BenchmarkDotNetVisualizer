using System.Dynamic;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// ExpandoObject Extensions
/// </summary>
public static partial class ExpandoObjectExtensions
{
    /// <summary>
    /// The column order prefix
    /// </summary>
    public const string ColumnOrderPrefix = "columns-order.";

    /// <summary>
    /// Gets the columns by order.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static string[] GetColumnsByOrder(this ExpandoObject expando)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));

        return expando.GetMetaProperties()
            .AsDictionary()
            .Where(p => p.Key.StartsWith(ColumnOrderPrefix))
            .OrderBy(p => (int)p.Value!)
            .Select(p => p.Key[ColumnOrderPrefix.Length..])
            .Where(expando.HasProperty)
            .ToArray();
    }

    /// <summary>
    /// Sets the columns order as specified.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="columnsOrder">The columns order.</param>
    public static void SetColumnsOrder(this IEnumerable<ExpandoObject?> enumerable, string[] columnsOrder)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(columnsOrder, nameof(columnsOrder));

        foreach (var expando in enumerable)
        {
            if (expando is null)
                continue;
            SetColumnsOrder(expando, columnsOrder);
        }
    }

    /// <summary>
    /// Sets the columns order as specified.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="columnsOrder">The columns order.</param>
    public static void SetColumnsOrder(this ExpandoObject expando, string[] columnsOrder)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentNullException.ThrowIfNull(columnsOrder, nameof(columnsOrder));

        for (int order = 0; order < columnsOrder.Length; order++)
        {
            var column = columnsOrder[order];
            expando.SetColumnOrder(column, order);
        }
    }

    /// <summary>
    /// Sets the column order identity (the order of the last column + 1).
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    public static void SetColumnOrderIdentity(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        var lastProperty = expando.AsDictionary().Last(p => p.Key != propertyName).Key;
        var order = expando.GetColumnOrder(lastProperty);
        expando.SetColumnOrder(propertyName, order + 1);
    }

    /// <summary>
    /// Transfers the column order from <paramref name="fromPropertyName"/> to <paramref name="toPropertyName"/>.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="fromPropertyName">Name of from property.</param>
    /// <param name="toPropertyName">Name of to property.</param>
    public static void TransferColumnOrder(this ExpandoObject expando, string fromPropertyName, string toPropertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(fromPropertyName, nameof(fromPropertyName));
        ArgumentException.ThrowIfNullOrWhiteSpace(toPropertyName, nameof(toPropertyName));

        var order = expando.GetColumnOrder(fromPropertyName);
        expando.SetColumnOrder(toPropertyName, order);
    }

    /// <summary>
    /// Sets the column order.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="order">The order.</param>
    public static void SetColumnOrder(this ExpandoObject expando, string propertyName, int order)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        expando.SetMetaProperty(ColumnOrderPrefix + propertyName, order);
    }

    /// <summary>
    /// Gets the column order.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static int GetColumnOrder(this ExpandoObject expando, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName, nameof(propertyName));

        return (int)expando.GetMetaProperty(ColumnOrderPrefix + propertyName)!;
    }
}
