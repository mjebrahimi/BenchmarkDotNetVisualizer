using System.Dynamic;
using System.Reflection;

namespace BenchmarkDotNetVisualizer.Utilities;

public static partial class DataProcessor
{
    public static IEnumerable<ExpandoObject?> JoinAndColorize<T>(this IEnumerable<IEnumerable<T?>> enumerable,
        string mainColumn, string[] groupByColumns, string pivotProperty, string statisticColumn, string[] orderColumns,
        bool colorizeStatisticColumn, bool colorizeGroups, bool addNullDividerBetweenGroups)
    {
        var expandoCollection = enumerable.Select(p => p.Select(p => p is null ? null : ExpandoObjectExtensions.ToExpandoObject(p)));
        return JoinAndColorize(expandoCollection, mainColumn, groupByColumns, pivotProperty, statisticColumn, orderColumns, colorizeStatisticColumn, colorizeGroups, addNullDividerBetweenGroups);
    }

    #region OrderByValues
    //internal static IOrderedEnumerable<IEnumerable<T?>> OrderByValues<T>(IEnumerable<IEnumerable<T?>> enumerable, string orderColumn, string[] orderValues)
    //{
    //    Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
    //    ArgumentNullException.ThrowIfNull(orderColumn, nameof(orderColumn));
    //    Guard.ThrowIfNullOrEmpty(orderValues, nameof(orderValues));

    //    var prop = typeof(T).GetProperty(orderColumn, BindingFlags.Instance | BindingFlags.Public);
    //    return enumerable.OrderBy(collection =>
    //    {
    //        var columnValue = prop!.GetValue(collection.First())!.ToString()!.RemoveMarkdownBold();
    //        var columnIndex = Array.IndexOf(orderValues, columnValue);
    //        return columnIndex == -1 ? int.MaxValue /*Put column at the end*/ : columnIndex;
    //    });
    //}

    //internal static IOrderedEnumerable<IEnumerable<T?>> OrderByValues<T>(IEnumerable<IEnumerable<T?>> enumerable, Func<T?, object?> orderColumn, string[] orderValues)
    //{
    //    Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
    //    ArgumentNullException.ThrowIfNull(orderColumn, nameof(orderColumn));
    //    Guard.ThrowIfNullOrEmpty(orderValues, nameof(orderValues));

    //    return enumerable.OrderBy(collection =>
    //    {
    //        var columnValue = orderColumn(collection.First())!.ToString()!.RemoveMarkdownBold();
    //        var columnIndex = Array.IndexOf(orderValues, columnValue);
    //        return columnIndex == -1 ? int.MaxValue /*Put column at the end*/ : columnIndex;
    //    });
    //}
    #endregion

    #region SortEachCollection
    //internal static IEnumerable<T?> SortEachCollection<T>(IEnumerable<T?> enumerable, params string[] orderColumns)
    //{
    //    Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
    //    Guard.ThrowIfNullOrEmpty(orderColumns, nameof(orderColumns));

    //    //Split collection by divider and Sort each collection by specified column, then concat all collections into one
    //    return enumerable
    //        .SplitBy(p => p is null)
    //        .Select(collection => collection.OrderByMultipleAuto(orderColumns))
    //        .ConcatBy(default);
    //}

    //internal static IEnumerable<T?> SortEachCollection<T>(IEnumerable<T?> enumerable, params Func<T?, object?>[] orderColumns)
    //{
    //    Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
    //    Guard.ThrowIfNullOrEmpty(orderColumns, nameof(orderColumns));

    //    //Split collection by divider and Sort each collection by specified column, then concat all collections into one
    //    return enumerable
    //        .SplitBy(p => p is null)
    //        .Select(collection => collection.OrderByMultipleAuto(orderColumns))
    //        .ConcatBy(default);
    //}
    #endregion

    #region ColorizeNumericColumn
    public static void ColorizeNumericColumn<T>(IEnumerable<T?> enumerable, params string[] numericColumns)
    {
        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(p => p.Name, p => p);
        foreach (var numericColumn in numericColumns)
        {
            foreach (var subset in enumerable.SplitByOccurrence(expando => expando is null))
            {
                var items = subset.Select(item =>
                {
                    var str = props[numericColumn].GetValue(item)!.ToString()!.RemoveMarkdownBold();
                    var value = str.ExtractNumberOrDefault();
                    props[numericColumn].SetValue(item, str); //Remove markdown bold
                    return new { Item = item, StatisticValue = value };
                }).ToArray();

                var min = items.Min(item => item.StatisticValue);
                var max = items.Max(item => item.StatisticValue);
                max = Math.Max(max, min * 2);
                var diff = max - min;

                foreach (var item in items)
                {
                    var relativeScore = diff == 0 ? 1 : (max - item.StatisticValue) / diff;
                    if (item.StatisticValue == min)
                    {
                        var str = props[numericColumn].GetValue(item.Item)!.ToString()!.RemoveMarkdownBold();
                        props[numericColumn].SetValue(item.Item, $"**{str}**"); //Set as markdown Bold
                    }

                    var color = ColorHelper.GetColorBetweenRedAndGreen(Convert.ToSingle(relativeScore));
                    color = ColorHelper.Lighten(color, 0.6);

                    item.Item!.SetMetaProperty($"{numericColumn}.background-color", color);
                }
            }
        }
    }
    #endregion

    #region ColorizeByGroup
    public static void ColorizeByGroup<T>(IEnumerable<T?> enumerable, params string[] groupByProperties)
    {
        ColorizeByGroup(enumerable, groupByProperties, LightCyanColor, LavenderPinkColor);
    }

    public static void ColorizeByGroup<T>(IEnumerable<T?> enumerable, string[] groupByProperties, string color1 = LightCyanColor, string color2 = LavenderPinkColor)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));
        ArgumentException.ThrowIfNullOrWhiteSpace(color1, nameof(color1));
        ArgumentException.ThrowIfNullOrWhiteSpace(color2, nameof(color2));

        var shoudChangeColor = false;
        string? previeusGroupValue = null;
        foreach (var item in enumerable)
        {
            var currentGroupValue = GenerateKeyFromPropertiesOrDefault(item, groupByProperties);
            if (item is null || (previeusGroupValue is not null && currentGroupValue!.Equals(previeusGroupValue) is false))
            {
                previeusGroupValue = currentGroupValue;
                shoudChangeColor ^= true;
                if (item is null)
                    continue;
            }
            previeusGroupValue = currentGroupValue;

            foreach (var propertyName in groupByProperties)
            {
                if (shoudChangeColor)
                    item.SetMetaProperty($"{propertyName}.background-color", color1);
                else
                    item.SetMetaProperty($"{propertyName}.background-color", color2);
            }
            //item.SetProperty("Method", $"**{item.GetPropertyOrDefault("Method")?.ToString()}**"); //bold Method column
        }
    }
    #endregion

    #region AddNullDividerBetweenGroups
    public static IEnumerable<T?> AddNullDividerBetweenGroups<T>(IEnumerable<T> enumerable, params string[] groupByProperties)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));

        return enumerable
            .Where(p => p is not null)
            .GroupBy(item => GenerateKeyFromPropertiesOrDefault(item, groupByProperties))
            .Select(p => p.AsEnumerable())
            .ConcatBy(default);
    }

    public static IEnumerable<T?> AddNullDividerBetweenGroups<T>(IEnumerable<T> enumerable, params Func<T, object?>[] keySelectors)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(keySelectors, nameof(keySelectors));

        return enumerable
            .Where(p => p is not null)
            .GroupBy(item => GenerateKeyFromPropertiesOrDefault(item, keySelectors))
            .Select(p => p.AsEnumerable())
            .ConcatBy(default);
    }
    #endregion

    #region GenerateKeyFromProperties
    public static string GenerateKeyFromProperties<T>(T item, params string[] keyProperties)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        Guard.ThrowIfNullOrEmpty(keyProperties, nameof(keyProperties));

        var props = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(p => p.Name, p => (Func<object?, object?>)p.GetValue);

        var propertyValues = keyProperties.Select(propertyName => props[propertyName].Invoke(item)?.ToString()!.Trim('*') ?? "");
        return string.Join('_', propertyValues);
    }

    public static string GenerateKeyFromProperties<T>(T item, params Func<T, object?>[] keySelectors)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        Guard.ThrowIfNullOrEmpty(keySelectors, nameof(keySelectors));

        var propertyValues = keySelectors.Select(prop => prop(item)?.ToString()!.Trim('*') ?? "");
        return string.Join('_', propertyValues);
    }

    public static string? GenerateKeyFromPropertiesOrDefault<T>(T? item, params string[] keyProperties)
    {
        Guard.ThrowIfNullOrEmpty(keyProperties, nameof(keyProperties));

        if (item is null)
            return null;

        var props = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(p => p.Name, p => (Func<object?, object?>)p.GetValue);
        var propertyValues = keyProperties.Select(propertyName => props.GetValueOrDefault(propertyName)?.Invoke(item)?.ToString()!.Trim('*') ?? "");
        return string.Join('_', propertyValues);
    }

    public static string? GenerateKeyFromPropertiesOrDefault<T>(T? item, params Func<T, object?>[] keySelectors)
    {
        Guard.ThrowIfNullOrEmpty(keySelectors, nameof(keySelectors));

        if (item is null)
            return null;

        var propertyValues = keySelectors.Select(prop => prop(item)?.ToString()!.Trim('*') ?? "");
        return string.Join('_', propertyValues);
    }
    #endregion
}
