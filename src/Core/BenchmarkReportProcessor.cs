using BenchmarkDotNetVisualizer.Utilities;
using Force.DeepCloner;
using System.Dynamic;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Benchmark Report Processor
/// </summary>
public static class BenchmarkReportProcessor
{
    #region JoinAndProcess/Process
    /// <summary>
    /// Joins the collections and process it for grouping, highlighting, spectrum colorizing, and sorting.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="mainColumn">The main column.</param>
    /// <param name="groupByColumns">The group by columns.</param>
    /// <param name="pivotProperty">The pivot property.</param>
    /// <param name="statisticColumn">The statistic column.</param>
    /// <param name="columnsOrder">The order columns.</param>
    /// <param name="otherColumnsToSelect">The other columns to select.</param>
    /// <param name="spectrumStatisticColumn">Spectrum statistic column if set to <c>true</c>.</param>
    /// <param name="highlightGroups">Highlights groups if set to <c>true</c>.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> JoinAndProcess(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable,
        string mainColumn, string[] groupByColumns, string pivotProperty, string statisticColumn, string[] columnsOrder,
        string[]? otherColumnsToSelect = null, bool spectrumStatisticColumn = true, bool highlightGroups = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentException.ThrowIfNullOrWhiteSpace(mainColumn, nameof(mainColumn));
        Guard.ThrowIfNullOrEmpty(groupByColumns, nameof(groupByColumns));
        ArgumentException.ThrowIfNullOrWhiteSpace(pivotProperty, nameof(pivotProperty));
        ArgumentException.ThrowIfNullOrWhiteSpace(statisticColumn, nameof(statisticColumn));
        Guard.ThrowIfNullOrEmpty(columnsOrder, nameof(columnsOrder));

        otherColumnsToSelect ??= ["Categories"];

        enumerable = enumerable.Select(innerCollection => innerCollection.Select(expando => expando?.CloneWithMetaProperties()).ToArray()).ToArray();

        enumerable = enumerable
            .MergeAndSplitByGroup(pivotProperty).ToArray();
        //.OrderCollectionsByValues(pivotProperty, columnsOrder).ToArray(); //Old approach for columns ordering

        enumerable = PivotColumnEachCollection(enumerable, pivotProperty, statisticColumn);

        string[] joinKeyColumns = [mainColumn, .. groupByColumns];
        var joined = JoinCollectionsTogether(enumerable, joinKeyColumns, columnsOrder);

        RemoveMarkdownBoldFromProperties(joined);

        var spectrumColumns = spectrumStatisticColumn ? columnsOrder : null;
        var result = Process(joined, groupByColumns, spectrumColumns, columnsOrder, highlightGroups, boldEntireRowOfLowestValue: false);

        string[] allColumnsByOrder = [mainColumn, .. otherColumnsToSelect, .. groupByColumns, .. columnsOrder];
        result.RemovePropertiesExcept(allColumnsByOrder);
        result.SetColumnsOrder(allColumnsByOrder); //The new approach is SetColumnsOrder

        return result;
    }

    /// <summary>
    /// Processes the specified enumerable for grouping, highlighting, spectrum colorizing, and sorting.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByColumns">The group by columns.</param>
    /// <param name="spectrumColumns">The spectrum columns.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <param name="highlightGroups">if set to <c>true</c> highlights groups.</param>
    /// <param name="boldEntireRowOfLowestValue">if set to <c>true</c> bolds the entire row of the lowest value.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> Process(this IEnumerable<ExpandoObject?> enumerable,
        string[]? groupByColumns = null, string[]? spectrumColumns = null, string[]? sortByColumns = null, bool highlightGroups = true, bool boldEntireRowOfLowestValue = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));

        // Commented because of better developer experience
        //if (highlightGroups && groupByColumns.IsNullOrEmpty())
        //{
        //    throw new System.ArgumentException(
        //        $"Argument '{nameof(highlightGroups)}' is set to true but '{nameof(groupByColumns)}' are not specified. " +
        //        $"Set '{nameof(highlightGroups)}' to false or provide '{nameof(groupByColumns)}", nameof(groupByColumns));
        //}

        enumerable = enumerable.Select(expando => expando?.CloneWithMetaProperties()).ToArray().AsEnumerable();
        enumerable.RemoveMarkdownBoldFromProperties();

        if (groupByColumns.IsNotNullOrEmpty())
        {
            enumerable = AddNullDividerBetweenEachGroup(enumerable, groupByColumns)!;

            if (highlightGroups)
                SplitByNullAndHighlightColumns(enumerable, groupByColumns); //Doesn't need to SplitByGroup because it already split by null
        }

        if (spectrumColumns.IsNotNullOrEmpty())
            enumerable = SplitByNullAndSpectrumColumns(enumerable, spectrumColumns, boldEntireRowOfLowestValue);

        if (sortByColumns.IsNotNullOrEmpty())
            enumerable = SplitByNullAndSortEachCollection(enumerable, sortByColumns);

        return enumerable;
    }
    #endregion

    #region OrderCollectionsByValues
    /// <summary>
    /// Orders the collections by values.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderColumn">The order column.</param>
    /// <param name="orderValues">The order values.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<IEnumerable<ExpandoObject?>> OrderCollectionsByValues(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, string orderColumn, string[] orderValues)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(orderColumn, nameof(orderColumn));
        Guard.ThrowIfNullOrEmpty(orderValues, nameof(orderValues));

        return enumerable.OrderBy(collection =>
        {
            var columnValue = collection.First(p => p is not null)!.GenerateKeyFromPropertiesOrDefault(orderColumn);
            var columnIndex = Array.IndexOf(orderValues, columnValue);
            return columnIndex == -1 ? int.MaxValue /*Put column at the end*/ : columnIndex;
        });
    }

#pragma warning disable S2368 // Public methods should not have multidimensional array parameters
    /// <summary>
    /// Orders the collections by values.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="orderColumns">The order columns.</param>
    /// <param name="orderValues">The order values.</param>
    /// <returns></returns>
    public static IOrderedEnumerable<IEnumerable<ExpandoObject?>> OrderCollectionsByValues(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, string[] orderColumns, string[][] orderValues)
#pragma warning restore S2368 // Public methods should not have multidimensional array parameters
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(orderColumns, nameof(orderColumns));
        Guard.ThrowIfNullOrEmpty(orderValues, nameof(orderValues));

        var stringOrderValues = orderValues.Select(arr => string.Join('_', arr)).ToArray();

        return enumerable.OrderBy(collection =>
        {
            var columnValue = collection.First(p => p is not null)!.GenerateKeyFromPropertiesOrDefault(orderColumns);
            var columnIndex = Array.IndexOf(stringOrderValues, columnValue);
            return columnIndex == -1 ? int.MaxValue /*Put column at the end*/ : columnIndex;
        });
    }
    #endregion

    #region Pivot
    /// <summary>
    /// Pivots the column each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="pivotColumn">The pivot column.</param>
    /// <param name="statisticColumn">The statistic column.</param>
    /// <param name="removeStatisticColumn">Remove statistic column if set to <c>true</c>.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject?>> PivotColumnEachCollection(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, string pivotColumn, string statisticColumn, bool removeStatisticColumn = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentException.ThrowIfNullOrWhiteSpace(pivotColumn, nameof(pivotColumn));
        ArgumentException.ThrowIfNullOrWhiteSpace(statisticColumn, nameof(statisticColumn));

        return enumerable.Select(innerCollection => PivotColumn(innerCollection, pivotColumn, statisticColumn, removeStatisticColumn));
    }

    /// <summary>
    /// Pivots the column.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="pivotColumn">The pivot column.</param>
    /// <param name="statisticColumn">The statistic column.</param>
    /// <param name="removeStatisticColumn">Remove statistic column if set to <c>true</c>.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> PivotColumn(this IEnumerable<ExpandoObject?> enumerable, string pivotColumn, string statisticColumn, bool removeStatisticColumn = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentException.ThrowIfNullOrWhiteSpace(pivotColumn, nameof(pivotColumn));
        ArgumentException.ThrowIfNullOrWhiteSpace(statisticColumn, nameof(statisticColumn));

        enumerable = enumerable.Select(expando => expando?.CloneWithMetaProperties()).ToArray().AsEnumerable();

        foreach (var expando in enumerable)
        {
            if (expando is null)
            {
                yield return expando;
                continue;
            }

            var pivotValue = expando.GetProperty(pivotColumn)?.ToString()?.RemoveMarkdownBold();
            expando.ChangePropertyName(statisticColumn, pivotValue!);
            expando.TransferColumnOrder(statisticColumn, pivotValue!);

            if (removeStatisticColumn)
                expando.RemoveProperty(pivotColumn);

            yield return expando;
        }
    }
    #endregion

    /// <summary>
    /// Joins the collections together.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="joinKeyColumns">The join key columns.</param>
    /// <param name="selectColumns">The select columns.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IEnumerable<ExpandoObject> JoinCollectionsTogether(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, string[] joinKeyColumns, string[] selectColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(joinKeyColumns, nameof(joinKeyColumns));
        Guard.ThrowIfNullOrEmpty(selectColumns, nameof(selectColumns));

        enumerable = enumerable.Select(innerCollection => innerCollection.Select(expando => expando?.CloneWithMetaProperties()).ToArray()).ToArray();

        return enumerable.Aggregate((current, next) =>
        {
            return current.Join(next,
                left => left.GenerateKeyFromPropertiesOrDefault(joinKeyColumns),
                right => right.GenerateKeyFromPropertiesOrDefault(joinKeyColumns),
                (left, right) =>
                {
                    if (left is null || right is null)
                        throw new InvalidOperationException(); //Just for eliminate null possibility

                    foreach (var column in selectColumns)
                    {
                        if (right.TryGetProperty(column, out var rightValue))
                        {
                            rightValue = rightValue!.ToString();
                            left.SetColumnOrderIdentity(column);
                            left.SetProperty(column, rightValue);
                        }
                    }

                    return left;
                });
        })!; //Not needed because of cloning -- .ToArray(); //Use ToArray() because it's a modifier operation and running more than once causes Exception
    }

    #region AddNullDivider
    /// <summary>
    /// Adds the null divider between each group.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> AddNullDividerBetweenEachGroup(this IEnumerable<ExpandoObject?> enumerable, params string[] groupByProperties)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));

        return enumerable
            .SplitByGroup(groupByProperties)
            .ConcatByNull();
    }

    /// <summary>
    /// Adds the null divider between each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> AddNullDividerBetweenEachCollection(this IEnumerable<IEnumerable<ExpandoObject>> enumerable)
    {
        return enumerable.ConcatByNull();
    }
    #endregion

    #region HighlightColumns
    /// <summary>
    /// The highlight color 1 (Defaults to light cyan color <see langword="#CCFFFF"/>)
    /// </summary>
    public static string HighlightColor1 { get; set; } = "#CCFFFF";
    /// <summary>
    /// The highlight color 2 (Defaults to lavender pink color <see langword="#FFCCFF"/>)
    /// </summary>
    public static string HighlightColor2 { get; set; } = "#FFCCFF";

    /// <summary>
    /// Splits the by group and highlight columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByGroupAndHighlightColumns(this IEnumerable<ExpandoObject?> enumerable, params string[] groupByProperties)
    {
        return SplitByGroupAndHighlightColumns(enumerable, groupByProperties, HighlightColor1, HighlightColor2);
    }

    /// <summary>
    /// Splits the by group and highlight columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <param name="color1">The color1.</param>
    /// <param name="color2">The color2.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByGroupAndHighlightColumns(this IEnumerable<ExpandoObject?> enumerable, string[] groupByProperties, string color1, string color2)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));
        ArgumentException.ThrowIfNullOrWhiteSpace(color1, nameof(color1));
        ArgumentException.ThrowIfNullOrWhiteSpace(color2, nameof(color2));

        var result = enumerable
            .SplitByGroup(groupByProperties)
            .HighlightColumnsOfEachCollection(groupByProperties, color1, color2);
        return enumerable.HasNullDivider() ? result.ConcatByNull() : result.Concat();

        //=========== Old Implementation ===========
        //var toggleColor = false;
        //string? previeusGroupValue = null;
        //foreach (var expando in enumerable)
        //{
        //    var currentGroupValue = expando.GenerateKeyFromPropertiesOrDefault(groupByProperties);
        //    if (expando is null || (previeusGroupValue is not null && currentGroupValue!.Equals(previeusGroupValue) is false))
        //    {
        //        previeusGroupValue = currentGroupValue;
        //        toggleColor ^= true;
        //        if (expando is null)
        //            continue;
        //    }
        //    previeusGroupValue = currentGroupValue;
        //
        //    foreach (var propertyName in groupByProperties)
        //    {
        //        if (toggleColor)
        //            expando.SetMetaProperty($"{propertyName}.background-color", color1);
        //        else
        //            expando.SetMetaProperty($"{propertyName}.background-color", color2);
        //    }
        //}
    }

    /// <summary>
    /// Splits the by null and highlight columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="colorizingColumns">The colorizing columns.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByNullAndHighlightColumns(this IEnumerable<ExpandoObject?> enumerable, params string[] colorizingColumns)
    {
        return SplitByNullAndHighlightColumns(enumerable, colorizingColumns, HighlightColor1, HighlightColor2);
    }

    /// <summary>
    /// Splits the by null and highlight columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="highlightColumns">The highlight columns.</param>
    /// <param name="color1">The color1.</param>
    /// <param name="color2">The color2.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByNullAndHighlightColumns(this IEnumerable<ExpandoObject?> enumerable, string[] highlightColumns, string color1, string color2)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(highlightColumns, nameof(highlightColumns));
        ArgumentException.ThrowIfNullOrWhiteSpace(color1, nameof(color1));
        ArgumentException.ThrowIfNullOrWhiteSpace(color2, nameof(color2));

        return enumerable
            .SplitByNull()
            .HighlightColumnsOfEachCollection(highlightColumns, color1, color2)
            .ConcatByNull();
    }

    /// <summary>
    /// Highlights the columns of each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="highlightColumns">The highlight columns.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject?>> HighlightColumnsOfEachCollection(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, params string[] highlightColumns)
    {
        return HighlightColumnsOfEachCollection(enumerable, highlightColumns, HighlightColor1, HighlightColor2);
    }

    /// <summary>
    /// Highlights the columns of each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="highlightColumns">The highlight columns.</param>
    /// <param name="color1">The color1.</param>
    /// <param name="color2">The color2.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject?>> HighlightColumnsOfEachCollection(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, string[] highlightColumns, string color1, string color2)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentException.ThrowIfNullOrWhiteSpace(color1, nameof(color1));
        ArgumentException.ThrowIfNullOrWhiteSpace(color2, nameof(color2));

        enumerable = enumerable.Select(innerCollection => innerCollection.Select(expando => expando?.CloneWithMetaProperties()).ToArray()).ToArray();

        var toggleColor = false;
        foreach (var innerCollection in enumerable)
        {
            foreach (var expando in innerCollection)
            {
                if (expando is null)
                    continue;

                foreach (var propertyName in highlightColumns)
                {
                    if (toggleColor)
                        expando.SetMetaProperty($"{propertyName}.background-color", color1);
                    else
                        expando.SetMetaProperty($"{propertyName}.background-color", color2);
                }
            }
            toggleColor ^= true;
            yield return innerCollection;
        }
    }
    #endregion

    #region SpectrumColumns
    /// <summary>
    /// The spectrum maximum threshold (Defaults to <see langword="2"/>).
    /// </summary>
    /// <remarks>
    /// Experiment with values between <see langword="1"/> and <see langword="3"/> and see which provides better output for your case.
    /// </remarks>
    public static decimal SpectrumMaxThreshold { get; set; } = 2;

    /// <summary>
    /// Gets or sets the get maximum function.
    /// </summary>
    /// <value>
    /// The get maximum function.
    /// </value>
    public static Func<string, decimal[], decimal>? GetMaximumFunc { get; set; }

    /// <summary>
    /// Splits the by group and spectrum columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <param name="spectrumColumns">The spectrum columns.</param>
    /// <param name="boldEntireRowOfLowestValue">if set to <c>true</c> bolds the entire row of the lowest value.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByGroupAndSpectrumColumns(this IEnumerable<ExpandoObject?> enumerable, string[] groupByProperties, string[] spectrumColumns, bool boldEntireRowOfLowestValue = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));
        Guard.ThrowIfNullOrEmpty(spectrumColumns, nameof(spectrumColumns));

        var result = enumerable
            .SplitByGroup(groupByProperties)
            .SpectrumColumns(spectrumColumns, boldEntireRowOfLowestValue);
        return enumerable.HasNullDivider() ? result.ConcatByNull() : result.Concat();
    }

    /// <summary>
    /// Splits the by null and spectrum columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="spectrumColumns">The spectrum columns.</param>
    /// <param name="boldEntireRowOfLowestValue">if set to <c>true</c> bolds the entire row of the lowest value.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByNullAndSpectrumColumns(this IEnumerable<ExpandoObject?> enumerable, string[] spectrumColumns, bool boldEntireRowOfLowestValue = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(spectrumColumns, nameof(spectrumColumns));

        return enumerable
            .SplitByNull()
            .SpectrumColumns(spectrumColumns, boldEntireRowOfLowestValue)
            .ConcatByNull();
    }

    /// <summary>
    /// Spectrum the columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="spectrumColumns">The spectrum columns.</param>
    /// <param name="boldEntireRowOfLowestValue">if set to <c>true</c> bolds the entire row of the lowest value.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject>> SpectrumColumns(this IEnumerable<IEnumerable<ExpandoObject>> enumerable, string[] spectrumColumns, bool boldEntireRowOfLowestValue = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(spectrumColumns, nameof(spectrumColumns));

        return enumerable.Select(innerCollection => SpectrumColumns(innerCollection, spectrumColumns, boldEntireRowOfLowestValue));
    }

    /// <summary>
    /// Spectrum the columns.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="spectrumColumns">The spectrum columns.</param>
    /// <param name="boldEntireRowOfLowestValue">if set to <c>true</c> bolds the entire row of the lowest value.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject> SpectrumColumns(this IEnumerable<ExpandoObject> enumerable, string[] spectrumColumns, bool boldEntireRowOfLowestValue = true)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(spectrumColumns, nameof(spectrumColumns));

        enumerable = enumerable.Select(expando => expando.CloneWithMetaProperties()).ToArray().AsEnumerable();

        var dictionary = spectrumColumns
            .Select(spectrumColumn =>
            {
                var statisticValues = enumerable.Select(expando =>
                {
                    var str = expando.GetProperty(spectrumColumn)!.ToString()!.RemoveMarkdownBold();
                    var statisticValue = str.ExtractNumberOrDefault();
                    expando.SetProperty(spectrumColumn, str); //Remove markdown bold
                    return statisticValue;
                }).ToArray();

                var min = statisticValues.Min();
                var max = GetMaximumFunc?.Invoke(spectrumColumn, statisticValues) ?? statisticValues.Max();

                max = Math.Max(max, min * SpectrumMaxThreshold);
                var diff = max - min;

                return (SpectrumColumn: spectrumColumn, MinMaxDiff: (Min: min, Max: max, Diff: diff));
            }).ToDictionary(p => p.SpectrumColumn, p => p.MinMaxDiff);

        foreach (var expando in enumerable)
        {
            var isFirstColumn = true;
            foreach (var spectrumColumn in spectrumColumns)
            {
                var (min, max, diff) = dictionary[spectrumColumn];
                var statisticValue = expando.GetProperty(spectrumColumn)!.ToString()!.RemoveMarkdownBold().ExtractNumberOrDefault();
                if (statisticValue > max) //for skipped maximums
                    statisticValue = max;

                var relativeScore = diff == 0 ? 1 : (max - statisticValue) / diff;
                if (statisticValue == min)
                {
                    if (isFirstColumn && boldEntireRowOfLowestValue)
                        expando.SetMarkdownBoldForProperties(); //Bolds the entire row of the lowest value
                    else
                        expando.SetMarkdownBoldForProperties(spectrumColumn); //Bolds only the lowest value cell
                }

                var color = ColorHelper.GetColorBetweenRedAndGreen(Convert.ToSingle(relativeScore));
                color = ColorHelper.Lighten(color, 0.6);

                expando.SetMetaProperty($"{spectrumColumn}.background-color", color);
                isFirstColumn = false;
            }
            yield return expando;
        }

        //Old implementation
        //var isFirstColumn = true;
        //foreach (var numericColumn in spectrumColumns)
        //{
        //    var items = enumerable.Select(expando =>
        //    {
        //        var str = expando.GetProperty(numericColumn)!.ToString()!.RemoveMarkdownBold();
        //        var value = str.ExtractNumberOrDefault();
        //        expando.SetProperty(numericColumn, str); //Remove markdown bold
        //        return new { Expando = expando, StatisticValue = value };
        //    }).ToArray();

        //    var min = items.Min(item => item.StatisticValue);
        //    var max = items.Max(item => item.StatisticValue);
        //    max = Math.Max(max, min * SpectrumMaxThreshold);
        //    var diff = max - min;

        //    foreach (var item in items)
        //    {
        //        var relativeScore = diff == 0 ? 1 : (max - item.StatisticValue) / diff;
        //        if (item.StatisticValue == min)
        //        {
        //            if (isFirstColumn && boldEntireRowOfLowestValue)
        //                item.Expando.SetMarkdownBoldForProperties(); //Bolds the entire row of the lowest value
        //            else
        //                item.Expando.SetMarkdownBoldForProperties(numericColumn); //Bolds only the lowest value cell
        //        }

        //        var color = ColorHelper.GetColorBetweenRedAndGreen(Convert.ToSingle(relativeScore));
        //        color = ColorHelper.Lighten(color, 0.6);

        //        item.Expando.SetMetaProperty($"{numericColumn}.background-color", color);
        //    }

        //    isFirstColumn = false;
        //}
        //return enumerable;
    }
    #endregion

    #region SortEachGroup/Collection
    /// <summary>
    /// Splits the by group and sort each group.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByGroupAndSortEachGroup(this IEnumerable<ExpandoObject?> enumerable, string[] groupByProperties, string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        //Split collection by group and Sort each collection by specified column, then concat all collections into one by null divider
        return enumerable
            .SplitByGroup(groupByProperties)
            .SortEachCollection(sortByColumns)
            .ConcatByNull();
    }

    /// <summary>
    /// Splits the by null and sort each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByNullAndSortEachCollection(this IEnumerable<ExpandoObject?> enumerable, params string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        //Split collection by divider and Sort each collection by specified column, then concat all collections into one by null divider
        return enumerable
            .SplitByNull()
            .SortEachCollection(sortByColumns)
            .ConcatByNull();
    }

    /// <summary>
    /// Sorts the each collection.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject?>> SortEachCollection(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, params string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        return enumerable
            .Select(collection => collection!.OrderByPropertiesAuto(sortByColumns));
    }

    /// <summary>
    /// Splits the by group and sort each group descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByGroupAndSortEachGroupDescending(this IEnumerable<ExpandoObject?> enumerable, string[] groupByProperties, string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        //Split collection by group and Sort each collection by specified column, then concat all collections into one by null divider
        return enumerable
            .SplitByGroup(groupByProperties)
            .SortEachCollectionDescending(sortByColumns)
            .ConcatByNull();
    }

    /// <summary>
    /// Splits the by null and sort each collection descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> SplitByNullAndSortEachCollectionDescending(this IEnumerable<ExpandoObject?> enumerable, params string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        //Split collection by divider and Sort each collection by specified column, then concat all collections into one by null divider
        return enumerable
            .SplitByNull()
            .SortEachCollectionDescending(sortByColumns)
            .ConcatByNull();
    }

    /// <summary>
    /// Sorts the each collection descending.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="sortByColumns">The sort by columns.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject?>> SortEachCollectionDescending(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, params string[] sortByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(sortByColumns, nameof(sortByColumns));

        return enumerable
            .Select(collection => collection!.OrderByPropertiesAutoDescending(sortByColumns));
    }
    #endregion

    #region SplitBy/ConcactByNull
    /// <summary>
    /// Merges the and split by group.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByColumns">The group by columns.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject>> MergeAndSplitByGroup(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable, params string[] groupByColumns)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByColumns, nameof(groupByColumns));

        return enumerable
            .Concat()
            .SplitByGroup(groupByColumns);
    }

    /// <summary>
    /// Splits the by group.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="groupByProperties">The group by properties.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject>> SplitByGroup(this IEnumerable<ExpandoObject?> enumerable, string[] groupByProperties)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        Guard.ThrowIfNullOrEmpty(groupByProperties, nameof(groupByProperties));

        return enumerable
            .Where(p => p is not null)
            .SplitByGroup(p => p.GenerateKeyFromPropertiesOrDefault(groupByProperties))!;
    }

    /// <summary>
    /// Splits the by null.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ExpandoObject>> SplitByNull(this IEnumerable<ExpandoObject?> enumerable)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));

        return enumerable
            .SplitByOccurrence(p => p is null)!;
    }

    /// <summary>
    /// Concat the by null.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> ConcatByNull(this IEnumerable<IEnumerable<ExpandoObject?>> enumerable)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));

        return enumerable
            .ConcatBy(null);
    }
    #endregion

    #region GenerateKeyFromProperties
    /// <summary>
    /// Generates the key from properties.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="keyProperties">The key properties.</param>
    /// <returns></returns>
    public static string GenerateKeyFromProperties(this ExpandoObject expando, params string[] keyProperties)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));
        Guard.ThrowIfNullOrEmpty(keyProperties, nameof(keyProperties));

        var propertyValues = keyProperties.Select(propertyName => expando.GetProperty(propertyName)?.ToString()!.RemoveMarkdownBold() ?? "");
        return string.Join('_', propertyValues);
    }

    /// <summary>
    /// Generates the key from properties or default.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="keyProperties">The key properties.</param>
    /// <returns></returns>
    public static string? GenerateKeyFromPropertiesOrDefault(this ExpandoObject? expando, params string[] keyProperties)
    {
        Guard.ThrowIfNullOrEmpty(keyProperties, nameof(keyProperties));

        if (expando is null)
            return null;

        var propertyValues = keyProperties.Select(propertyName => expando.GetPropertyOrDefault(propertyName)?.ToString()!.RemoveMarkdownBold() ?? "");
        return string.Join('_', propertyValues);
    }
    #endregion

    #region RemoveMarkdownBoldFromAllProperties
    /// <summary>
    /// Removes the markdown bold from all or specified properties.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns></returns>
    public static void RemoveMarkdownBoldFromProperties(this IEnumerable<ExpandoObject?> enumerable, params string[] propertyNames)
    {
        Guard.ThrowIfNullOrEmpty(enumerable, nameof(enumerable));
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        //NOTE: may needed => enumerable = enumerable.Select(expando => expando?.CloneWithMetaProperties()).ToArray().AsEnumerable(); and return enumerable; as returned object

        foreach (var expando in enumerable)
        {
            if (expando is null)
                continue;

            RemoveMarkdownBoldFromProperties(expando, propertyNames);
        }
    }

    /// <summary>
    /// Removes the markdown bold from all or specified properties.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns></returns>
    public static void RemoveMarkdownBoldFromProperties(this ExpandoObject expando, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));

        //NOTE: may needed => expando = expando.CloneWithMetaProperties(); and return expando; as returned object

#pragma warning disable IDE0305 // Simplify collection initialization
        var props = propertyNames.Length > 0 ? propertyNames : expando.AsDictionary()!.Keys.Select(p => p).ToArray(); //Select is necessary
#pragma warning restore IDE0305 // Simplify collection initialization
        foreach (var prop in props!)
        {
            var value = expando.GetProperty(prop)?.ToString()?.RemoveMarkdownBold();
            expando.SetProperty(prop, value);
        }
    }

    /// <summary>
    /// Sets the markdown bold for all or specified properties.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <returns></returns>
    public static void SetMarkdownBoldForProperties(this ExpandoObject expando, params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(expando, nameof(expando));

        //NOTE: may needed => expando = expando.CloneWithMetaProperties(); and return expando; as returned object

#pragma warning disable IDE0305 // Simplify collection initialization
        var props = propertyNames.Length > 0 ? propertyNames : expando.AsDictionary()!.Keys.Select(p => p).ToArray(); //Select is necessary
#pragma warning restore IDE0305 // Simplify collection initialization
        foreach (var prop in props!)
        {
            var value = expando.GetProperty(prop)?.ToString()?.RemoveMarkdownBold();
            expando.SetProperty(prop, $"**{value}**");
        }
    }
    #endregion

    #region ExpandoObject Utils
    /// <summary>
    /// Determines whether the specified enumerable has (is split by) null divider.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>
    ///   <c>true</c> if the specified enumerable has (is split by) null divider; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasNullDivider(this IEnumerable<ExpandoObject?> enumerable) //IsSplitByNullDivider
    {
        return enumerable.Any(p => p is null);
    }

    /// <summary>
    /// Creates a clone with meta properties included.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <returns></returns>
    public static ExpandoObject CloneWithMetaProperties(this ExpandoObject expando)// where T : IDynamicMetaObjectProvider?, IDictionary<string, object?>? //ExpandoObject
    {
        var newItem = expando.DeepClone();
        newItem!.SetMetaProperties(expando.GetMetaProperties());
        return newItem;
    }
    #endregion
}