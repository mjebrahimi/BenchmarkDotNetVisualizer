using System.Data;
using System.Dynamic;
using System.Net;
using System.Reflection;
using System.Text;
using PropInfo = (string Name, System.Func<object?, object?> GetValue, System.Type Type);

namespace BenchmarkDotNetVisualizer.Utilities;

public static partial class MarkdownHelper
{
    public static IEnumerable<T?> ParseMarkdownTable<T>(string markdownTable, ParseTableDividerMode rowDivider = ParseTableDividerMode.PlaceNull) where T : new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(markdownTable, nameof(markdownTable));

        // Extract the table rows from the Markdown table
        var rows = markdownTable.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        if (rows.Length < 3)
        {
            // The table should have at least three rows (header, separator, and content)
            throw new System.ArgumentException("Invalid Markdown table format.");
        }

        return Iterate();

        IEnumerable<T?> Iterate()
        {
            // Extract the header row and determine the column names
            var headers = rows[0].Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Extract the separator row to determine column widths
            var separators = rows[1].Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);
            var columnWidths = new int[headers.Length];
            for (int i = 0; i < separators.Length; i++)
            {
                columnWidths[i] = separators[i].Trim().Length;
            }

            // Process the content rows
            for (int i = 2; i < rows.Length; i++)
            {
                var cells = rows[i].Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);

                if (cells.Length != headers.Length)
                {
                    // The number of cells in the row should match the number of columns
                    throw new System.ArgumentException("Invalid Markdown table format.");
                }

                var obj = new T();

                for (int j = 0; j < headers.Length; j++)
                {
                    var propertyName = headers[j].Trim();
                    var cellValue = cells[j].Trim();

                    cellValue = WebUtility.HtmlDecode(cellValue).Trim('\''); //because of the additional html encoded quotes (')(&#39;) in "Method" column

                    var property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                    if (property is not null)
                    {
                        var value = Convert.ChangeType(cellValue, property.PropertyType);
                        // Use reflection to set the property value dynamically
                        property.SetValue(obj, value);
                    }
                }

                if (IsDivider_IsAllPropsNullOrEmpty(obj))
                {
                    switch (rowDivider)
                    {
                        case ParseTableDividerMode.PlaceNull:
                            yield return default;
                            break;
                        case ParseTableDividerMode.Ignore:
                            continue;
                    }
                }
                else
                {
                    yield return obj;
                }
            }
        }
    }

    public static Task SaveAsMarkdownTableAsync<T>(IEnumerable<T?> source, string path, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToMarkdownTable(source, dividerMode);
        return File.WriteAllTextAsync(path, text);
    }

    public static void SaveAsMarkdownTable<T>(IEnumerable<T?> source, string path, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToMarkdownTable(source, dividerMode);
        File.WriteAllText(path, text);
    }

    public static string ToMarkdownTable<T>(IEnumerable<T?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        return source switch
        {
            IEnumerable<IEnumerable<ExpandoObject>> collectionExpando => ToMarkdownTableCore(collectionExpando.ConcatBy(null), dividerMode),
            IEnumerable<IEnumerable<object>> collectionT => ToMarkdownTableCore(collectionT.ConcatBy(null), dividerMode),
            _ => ToMarkdownTableCore(source, dividerMode)
        };
    }

    private static string ToMarkdownTableCore<T>(IEnumerable<T?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
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
                foreach (var table in source.SplitByOccurrence(p => p is null))
                    Render(table);
                break;
            default:
                throw new NotImplementedException();
        }
        return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());

        void Render(IEnumerable<T?> table)
        {
            var properties = typeof(T).GetRuntimeProperties();
            var fields = typeof(T).GetRuntimeFields().Where(p => p.IsPublic);

            var getterInfos = properties.Select(p => new PropInfo(p.Name, p.GetValue, p.PropertyType))
                             .Union(fields.Select(p => new PropInfo(p.Name, p.GetValue, p.FieldType)))
                             .ToArray();

            var maxColumnValues = table
                .Select(item => getterInfos.Select(propInfo => propInfo.GetValue(item)?.ToString()?.Length ?? 0))
                .Union(new[] { getterInfos.Select(propInfo => propInfo.Name.Length) }) // Include header in column sizes
                .Aggregate(
                    new int[getterInfos.Length].AsEnumerable(),
                    (accumulate, x) => accumulate.Zip(x, Math.Max))
                .ToArray();

            var columnNames = getterInfos.Select(propInfo => propInfo.Name);

            var headerValues = columnNames.Select((column, index) => column.PadRight(maxColumnValues[index]));
            var headerRow = string.Join(" | ", headerValues);
            var headerLine = $"| {headerRow} |";

            var headerDividerValues = getterInfos.Select((propInfo, index) => new string('-', maxColumnValues[index]) + (propInfo.Type.IsNumericType() ? ':' : ' '));
            var headerDividerRow = string.Join("| ", headerDividerValues);
            var headerDividerLine = $"| {headerDividerRow}|";

            var tableLines = table.Select(item =>
            {
                if (item is null && dividerMode is RenderTableDividerMode.Ignore)
                    return null;

                var columnValues = getterInfos.Select((propInfo, index) =>
                {
                    var columnValue = item is null ? "" : propInfo.GetValue(item)?.ToString() ?? "";
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

    public static bool IsDivider_IsAllPropsNullOrEmpty<T>(T obj)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        return properties.TrueForAll(prop => string.IsNullOrEmpty(prop.GetValue(obj)?.ToString()));
    }
}