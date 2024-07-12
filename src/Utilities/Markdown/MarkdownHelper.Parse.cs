using System.Dynamic;
using System.Net;
using System.Text.RegularExpressions;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Markdown Helper for Parsing
/// </summary>
public static partial class MarkdownHelper
{
    /// <summary>
    /// The line separator for all operation systems
    /// </summary>
    internal static readonly string[] separator = ["\r\n", "\r", "\n"];

    /// <summary>
    /// Parses the markdown table.
    /// </summary>
    /// <param name="markdownTable">The markdown table.</param>
    /// <param name="rowDivider">The row divider.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Invalid Markdown table format.</exception>
    public static IEnumerable<ExpandoObject?> ParseMarkdownTable(string markdownTable, ParseTableDividerMode rowDivider = ParseTableDividerMode.PlaceNull)
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

        IEnumerable<ExpandoObject?> Iterate()
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

                var expando = new ExpandoObject();

                for (int j = 0; j < headers.Length; j++)
                {
                    var propertyName = headers[j].Trim();
                    var cellValue = cells[j].Trim();

                    var isBold = IsSurroundedByMarkdownBold(cellValue);
                    if (isBold)
                        cellValue = cellValue.Trim('*');

                    cellValue = WebUtility.HtmlDecode(cellValue).Trim('\''); //because of the additional html encoded quotes (')(&#39;) in "Method" column

                    if (isBold)
                        cellValue = $"**{cellValue}**";

                    expando.SetProperty(propertyName, cellValue);

                    expando.SetColumnOrder(propertyName, j);
                }

                if (expando.IsNullDivider())
                {
                    switch (rowDivider)
                    {
                        case ParseTableDividerMode.PlaceNull:
                            yield return null;
                            break;
                        case ParseTableDividerMode.Ignore:
                            continue;
                    }
                }
                else
                {
                    yield return expando;
                }
            }
        }
    }

    /// <summary>
    /// Determines whether the specified expando is null divider.
    /// </summary>
    /// <param name="expando">The expando.</param>
    /// <returns>
    ///   <c>true</c> if the specified expando is null divider; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullDivider(this ExpandoObject expando)
    {
        if (expando is null)
            return true;

        var expandoDict = expando.AsDictionary()!;
        return expandoDict.Values.All(p => string.IsNullOrEmpty(p?.ToString()));
    }

    /// <summary>
    /// Removes the markdown bold from text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static string RemoveMarkdownBold(this string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        return GetMarkdownBoldRegex().Replace(text, "$1");
    }

    /// <summary>
    /// Determines whether the specified value is surrounded by markdown bold.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified value is surrounded by markdown bold; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsSurroundedByMarkdownBold(string? value)
    {
        if (value is null)
            return false;

        return value.StartsWith("**") && value.EndsWith("**");
    }

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"\*\*(.*?)\*\*")]
    internal static partial Regex GetMarkdownBoldRegex();
#else
    internal static Regex GetMarkdownBoldRegex() => MarkdownBoldRegex;
    internal static readonly Regex MarkdownBoldRegex = new(@"\*\*(.*?)\*\*");
#endif
}