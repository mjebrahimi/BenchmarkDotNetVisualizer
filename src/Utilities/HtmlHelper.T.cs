using System.Dynamic;
using System.Reflection;
using System.Text;

namespace BenchmarkDotNetVisualizer.Utilities;

public static partial class HtmlHelper
{
    public static Task RenderToImageAsync<T>(IEnumerable<T?> source, string path, string elementQuery = "body",
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, HtmlDocumentWrapMode.WithCSS);
        return RenderHtmlToImageAsync(html, path, elementQuery);
    }

    public static Task<byte[]> RenderToImageDataAsync<T>(IEnumerable<T?> source, string elementQuery = "body",
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(elementQuery, nameof(elementQuery));

        var table = ToHtmlTable(source, dividerMode);
        var html = WrapInHtmlDocument(table, string.Empty, HtmlDocumentWrapMode.WithCSS);
        return RenderHtmlToImageDataAsync(html, elementQuery);
    }

    public static Task SaveAsHtmlTableDocumentAsync<T>(IEnumerable<T?> source, string path, string title,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.SeparateTables, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.WithRichDataTables, CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title, dividerMode, htmlWrapMode);
        return File.WriteAllTextAsync(path, text, cancellationToken);
    }

    public static void SaveAsHtmlTableDocument<T>(IEnumerable<T?> source, string path, string title,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.SeparateTables, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.WithRichDataTables)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title, dividerMode, htmlWrapMode);
        File.WriteAllText(path, text);
    }

    public static string ToHtmlTableDocument<T>(IEnumerable<T?> source, string title,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.SeparateTables, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.WithRichDataTables)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        var table = ToHtmlTable(source, dividerMode);
        return WrapInHtmlDocument(table, title, htmlWrapMode);
    }

    public static string ToHtmlTable<T>(IEnumerable<T?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.SeparateTables)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        return source switch
        {
            IEnumerable<IEnumerable<ExpandoObject>> collectionExpando => ToHtmlTableCore(collectionExpando.ConcatBy(null), dividerMode),
            IEnumerable<IEnumerable<object>> collectionT => ToHtmlTableCore(collectionT.ConcatBy(null), dividerMode),
            _ => ToHtmlTableCore(source, dividerMode)
        };
    }

    private static string ToHtmlTableCore<T>(IEnumerable<T?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.SeparateTables)
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
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
            stringBuilder.AppendLine("<table>");

            //Header
            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            foreach (var prop in props)
            {
                stringBuilder.AppendLine($"<th>{prop.Name}</th>");
            }
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            //Body
            stringBuilder.AppendLine("<tbody>");
            foreach (var item in table)
            {
                if (item is null)
                {
                    switch (dividerMode)
                    {
                        case RenderTableDividerMode.EmptyDividerRow:
                            stringBuilder.AppendLine(@"<tr class=""divier"">");
                            stringBuilder.AppendLine($@"<td colspan=""{props.Length}"">&nbsp;</td>");
                            stringBuilder.AppendLine("</tr>");

                            stringBuilder.AppendLine(@"<tr class=""divier"">");
                            stringBuilder.AppendLine($@"<td colspan=""{props.Length}""></td>");
                            stringBuilder.AppendLine("</tr>");
                            continue;
                        case RenderTableDividerMode.Ignore:
                            continue;
                        default:
                            throw new NotImplementedException();
                    }
                }

                stringBuilder.AppendLine("<tr>");
                foreach (var prop in props)
                {
                    var value = ReplaceMarkdownBoldWithHtmlBold(prop.GetValue(item)?.ToString());
                    var style = item.TryGetMetaProperty($"{prop.Name}.background-color", out var bgColor) ? $@" style=""background-color: {bgColor};""" : null;

                    stringBuilder.AppendLine($"<td{style}>{value}</td>");
                }
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine();
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call
        }
    }
}
