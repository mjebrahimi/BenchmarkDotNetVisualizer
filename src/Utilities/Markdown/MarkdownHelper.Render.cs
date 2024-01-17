using System.Data;
using System.Dynamic;
using System.Text;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Markdown Helper for Rendering
/// </summary>
public static partial class MarkdownHelper
{
    /// <summary>
    /// Converts the enumerable to markdown table and saves as specified <paramref name="path"/> asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsMarkdownTableAsync(this IEnumerable<ExpandoObject?> source, string path, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow,
        CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = source.ToMarkdownTable(dividerMode);
        return File.WriteAllTextAsync(path, text, cancellationToken);
    }

    /// <summary>
    /// Converts the enumerable to markdown table and saves as specified <paramref name="path"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="dividerMode">The divider mode.</param>
    public static void SaveAsMarkdownTable(this IEnumerable<ExpandoObject?> source, string path, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = source.ToMarkdownTable(dividerMode);
        File.WriteAllText(path, text);
    }

    /// <summary>
    /// Converts the enumerable to markdown table.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <returns></returns>
    public static string ToMarkdownTable(this IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        return ToMarkdownTableCore(source, dividerMode);
    }

    private static string ToMarkdownTableCore(this IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        var stringBuilder = new StringBuilder();
        switch (dividerMode)
        {
            case RenderTableDividerMode.Ignore:
            case RenderTableDividerMode.EmptyDividerRow:
            case var _ when source.Any(p => p is null) is false:
                Render(source);
                break;
            case RenderTableDividerMode.SeparateTables:
                foreach (var table in source.SplitByNull())
                    Render(table);
                break;
            default:
                throw new NotImplementedException();
        }
        return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());

        void Render(IEnumerable<ExpandoObject?> table)
        {
            var columnNames = table.First()!.GetColumnsByOrder();

            var maxColumnValues = table.Where(expando => expando is not null)
                .Select(expando => columnNames.Select(column => expando?.GetProperty(column)?.ToString()?.Length ?? 0))
                .Union(new[] { columnNames.Select(column => column.Length) }) // Include header in column sizes
                .Aggregate(
                    new int[columnNames.Length].AsEnumerable(),
                    (accumulate, x) => accumulate.Zip(x, Math.Max))
                .ToArray();

            var headerValues = columnNames.Select((column, index) => column.PadRight(maxColumnValues[index]));
            var headerRow = string.Join(" | ", headerValues);
            var headerLine = $"| {headerRow} |";

            var isNumericValues = columnNames.Select(column => table.Any(item => item?.GetProperty(column)?.IsNumericType() is true)).ToArray();

            var headerDividerValues = columnNames.Select((_, index) => new string('-', maxColumnValues[index]) + (isNumericValues[index] ? ':' : ' '));
            var headerDividerRow = string.Join("| ", headerDividerValues);
            var headerDividerLine = $"| {headerDividerRow}|";

            var tableLines = table.Select(expando =>
            {
                if (expando is null && dividerMode is RenderTableDividerMode.Ignore)
                    return null;

                var columnValues = columnNames.Select((column, index) =>
                {
                    var columnValue = expando is null ? "" : expando.GetProperty(column)?.ToString() ?? "";
                    return columnValue.PadRight(maxColumnValues[index]);
                });
                var row = string.Join(" | ", columnValues);
                return $"| {row} |";
            }).Where(p => p is not null);

            stringBuilder.AppendLine(headerLine);
            stringBuilder.AppendLine(headerDividerLine);
            stringBuilder.AppendJoin(Environment.NewLine, tableLines);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }
    }

    /// <summary>
    /// Wraps the code in a markdown code block.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="language">The language.</param>
    /// <returns></returns>
    public static string WrapInCodeBlock(string code, string language = "text")
    {
        return $"""
                ```{language}
                {code}
                ```
                """;
    }

    /// <summary>
    /// Determines whether the specified object is numeric type.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///   <c>true</c> if the specified object is numeric type; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNumericType(this object? obj)
    {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous - ?.Trim('*')
        return obj?.GetType()?.IsNumericType() is true || obj?.ToString()?.Trim('*')?.StartsWithNumber() is true;
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
    }
}