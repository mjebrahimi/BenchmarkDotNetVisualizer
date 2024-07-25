using System.ComponentModel;
using System.Dynamic;
using System.Net;
using System.Text;
using BenchmarkDotNetVisualizer.Utilities.Html;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Html Helper for rendering html
/// </summary>
public static partial class HtmlHelper
{
    /// <summary>
    /// Converts the enumerable to HTML table document and saves as specified <paramref name="path"/> asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsHtmlTableDocumentAsync(this IEnumerable<ExpandoObject?> source, string path, string title,HtmlThemeOptions themeOption,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple,
        CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title,themeOption, dividerMode, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, text, cancellationToken);
    }

    /// <summary>
    /// Converts the enumerable to HTML table document and saves as specified <paramref name="path"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    public static void SaveAsHtmlTableDocument(this IEnumerable<ExpandoObject?> source, string path, string title,HtmlThemeOptions themeOption,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title,themeOption, dividerMode, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        File.WriteAllText(path, text);
    }

    /// <summary>
    /// Converts specified enumerable to HTML table document.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <returns></returns>
    public static string ToHtmlTableDocument(this IEnumerable<ExpandoObject?> source, string title,HtmlThemeOptions themeOption,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        var table = ToHtmlTable(source, dividerMode);
        return WrapInHtmlDocument(table, title,themeOption, htmlWrapMode);
    }

    /// <summary>
    /// Wraps the body in HTML document and save as specified <paramref name="path"/> asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The theme option</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task WrapInHtmlDocumentAndSaveAsAsync(string path, string body, string title,HtmlThemeOptions themeOption, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple,
        CancellationToken cancellationToken = default)
    {
        var html = WrapInHtmlDocument(body, title,themeOption, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Wraps the body in HTML document and save as specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The theme option</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    public static void WrapInHtmlDocumentAndSaveAs(string path, string body, string title,HtmlThemeOptions themeOption, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        var html = WrapInHtmlDocument(body, title,themeOption, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        File.WriteAllText(path, html);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the powered-by tag is disabled. (Defaults to <see langword="false"/>)
    /// </summary>
    /// <value>
    ///   <c>true</c> if you want to disable powered-by tag; otherwise, <c>false</c>.
    /// </value>
    public static bool DisablePoweredBy { get; set; }
    private static string GetPoweredBy()
    {
        return DisablePoweredBy ? string.Empty :
            """<div class="powered-by">Powered by <a href="https://github.com/mjebrahimi/BenchmarkDotNetVisualizer">https://github.com/mjebrahimi/BenchmarkDotNetVisualizer</a></div>""";
    }

    /// <summary>
    /// Wraps the body in HTML document and gets the output html.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="themeOption">The HTML theme mode</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException">nameof(htmlWrapMode), Convert.ToInt32(htmlWrapMode), typeof(HtmlDocumentWrapMode)</exception>
    public static string WrapInHtmlDocument(string body, string title,HtmlThemeOptions themeOption, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        
        var css = SetupCssStylingBasedOnThemeOption(themeOption);
        
#pragma warning disable IDE0066 // Convert switch statement to expression
        switch (htmlWrapMode)
        {
            case HtmlDocumentWrapMode.Simple:
                return
                    $$$"""
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                    <meta charset='utf-8' />
                    <title>{{{title}}}</title>
                    {{{css}}}
                    </head>
                    <body>
                    {{{body}}}
                    {{{GetPoweredBy()}}}
                    </body>
                    </html>
                    """;
            case HtmlDocumentWrapMode.RichDataTables:
                return
                    $$$"""
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                    <meta charset='utf-8' />
                    <title>{{{title}}}</title>
                    {{{css}}}
                    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.7/css/jquery.dataTables.min.css" />
                    <link rel="stylesheet" href="https://cdn.datatables.net/colreorder/1.5.4/css/colReorder.dataTables.min.css">
                    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/2.4.2/css/buttons.dataTables.min.css">
                    <style type="text/css">
                        input[type="search"] { font-size: 1em !important; padding: 9px 12px !important; }
                        thead th:hover { background-color: #E0E0E0; }
                    </style>
                    </head>
                    <body>
                    {{{body}}}
                    {{{GetPoweredBy()}}}
                    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
                    <script src="https://cdn.datatables.net/1.13.7/js/jquery.dataTables.min.js"></script>
                    <script src="https://cdn.datatables.net/colreorder/1.5.4/js/dataTables.colReorder.min.js"></script>
                    <script src="https://cdn.datatables.net/buttons/2.4.2/js/dataTables.buttons.min.js"></script>
                    <script src="https://cdn.datatables.net/buttons/2.4.2/js/buttons.colVis.min.js"></script>
                    </body>
                    <script>
                        $(document).ready( function () {
                            DataTable.ext.type.order['custom-orderer-pre'] = function (d) {
                                d = d.replace(/(<([^>]+)>)/gi, ""); //remove html tags
                                if (!d.match(/^\d/)) //if not started with number
                                    return d;
                                var regex = /[\d,]+(?:\.\d+)?/;
                                var number = d.match(regex)[0].replace(/,/g, ""); //extract number
                                return parseFloat(number); //Number
                            };
                            $('table').DataTable({
                                info: false,
                                paging: false,
                                colReorder: true,
                                order: [], //remove default order
                                columnDefs: [
                                    {
                                        type: 'custom-orderer',
                                        targets: '_all'
                                    }
                                ],
                                dom: 'Bfrtip',
                                buttons: ['colvis']
                            });
                        });
                    </script>
                    </html>
                    """;
            default:
                throw new InvalidEnumArgumentException(nameof(htmlWrapMode), Convert.ToInt32(htmlWrapMode), typeof(HtmlDocumentWrapMode));
        }
#pragma warning restore IDE0066 // Convert switch statement to expression
    }

    private static string SetupCssStylingBasedOnThemeOption(HtmlThemeOptions themeOption)
    {
        const string BRIGHTCSSFORMAT = """
                                       <style type="text/css">
                                                    body { font-family: system-ui; padding: 0 30px 30px 30px; width: min-content; display: inline-block; }
                                                    h1, h2 { border-bottom: solid 1px #D8DEE4;}
                                                    h1, h2, h3 { padding-bottom: 0.3em; }
                                                    table { border-collapse: collapse !important; margin-top: 3px !important; width: 100%; display: inline-table; margin-bottom: 20px !important; }
                                                    td, th { padding: 6px 13px; border: 1px solid #CCC; text-align: right; white-space: nowrap; }
                                                    tr { background-color: #FFF !important; border-top: 1px solid #CCC; }
                                                    tr:nth-child(even):not(.divider) { background: #F8F8F8 !important; }
                                                    tr.divider { border: 0; font-size: 10px; }
                                                    tr.divider td { padding: 0; border: 0; }
                                                    pre { background: #EFEFEF; padding: 0 1em; }
                                                    thead th { background-color: #EFEFEF; }
                                                    tbody tr:not(.divider):hover { background-color: #EFEFEF !important; border: 2px solid #ADADAD; }
                                                    .powered-by { text-align: center; margin-bottom: -20px !important; font-weight: bold; }
                                                </style>
                                       """;

        const string DARKCSSFORMAT = """
                                     <style type="text/css">
                                         body { font-family: system-ui; padding: 0 30px 30px 30px; width: min-content; display: inline-block; background-color: #121212; color: #E0E0E0; }
                                         h1, h2 { border-bottom: solid 1px #444444; }
                                         h1, h2, h3 { padding-bottom: 0.3em; }
                                         table { border-collapse: collapse !important; margin-top: 3px !important; width: 100%; display: inline-table; margin-bottom: 20px !important; }
                                         td, th { padding: 6px 13px; border: 1px solid #444444; text-align: right; white-space: nowrap; }
                                         tr { background-color: #1E1E1E !important; border-top: 1px solid #CCC; }
                                         tr:nth-child(even):not(.divider) { background: #444444 !important; }
                                         tr.divider { border: 0; font-size: 10px; }
                                         tr.divider td { padding: 0; border: 0; }
                                         pre { background: #2E2E2E; padding: 0 1em; color: #E0E0E0; }
                                         thead th { background-color: #2E2E2E; color: #E0E0E0; }
                                         tbody tr:not(.divider):hover { background-color: #333333 !important; border: 2px solid #777777; }
                                         .powered-by { text-align: center; margin-bottom: -20px !important; font-weight: bold; }
                                     </style>
                                     """
            ;

        return themeOption == HtmlThemeOptions.Bright ? BRIGHTCSSFORMAT : DARKCSSFORMAT;
    }

    /// <summary>
    /// Converts specified enumerable to HTML table (without whole document).
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <returns></returns>
    public static string ToHtmlTable(this IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        return ToHtmlTableCore(source, dividerMode);
    }

    private static string ToHtmlTableCore(IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
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
            var props = source.First()!.GetColumnsByOrder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
            stringBuilder.AppendLine("<table>");

            //Header
            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            foreach (var prop in props)
            {
                stringBuilder.Append($"<th>{prop}</th>");
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
                            stringBuilder.AppendLine(@"<tr class=""divider"">");
                            stringBuilder.AppendLine($@"<td colspan=""{props.Length}"">&nbsp;</td>");
                            stringBuilder.AppendLine("</tr>");

                            stringBuilder.AppendLine(@"<tr class=""divider"">");
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
                    var value = WebUtility.HtmlEncode(item.GetProperty(prop)?.ToString());

                    var styles = new List<string>();
                    if (value?.IsNumericType() is false)
                        styles.Add("text-align: left");

                    if (item.TryGetMetaProperty($"{prop}.background-color", out var backgroundColor))
                        styles.Add($"background-color: {backgroundColor}");

                    var style = styles.Count > 0 ? $@" style=""{string.Join("; ", styles)}""" : null;

                    value = ReplaceMarkdownBoldWithHtmlBold(value);
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

    /// <summary>
    /// Wraps the code in a <![CDATA[<pre><code></code></pre>]]> block.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns></returns>
    public static string WrapInCodeBlock(string code)
    {
        return $"""
                <pre>
                <code>
                {code}
                </code>
                </pre>
                """;
    }

    /// <summary>
    /// Replaces the markdown bold (**Bold**) to HTML bold (<![CDATA[<strong>Bold</strong>]]>).
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static string? ReplaceMarkdownBoldWithHtmlBold(string? text)
    {
        if (text is null)
            return null;

        return MarkdownHelper.GetMarkdownBoldRegex().Replace(text, "<strong>$1</strong>"); //"<b>$1</b>"
    }
}