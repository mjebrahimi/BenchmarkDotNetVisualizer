using BenchmarkDotNet.Reports;
using BenchmarkDotNetVisualizer.Utilities;
using System.Text;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Benchmark Visualizer
/// </summary>
public static partial class BenchmarkVisualizer
{
#pragma warning disable S4136 // Method overloads should be grouped together

    #region Html
    #region IEnumerable<Summary>
    /// <summary>
    /// Concatenates the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsHtmlAndImageAsync(this IEnumerable<Summary> summaries, string htmlPath, string imagePath,
        ConcatReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndSaveAsHtmlAndImageAsync(htmlPath, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and save as image asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsImageAsync(this IEnumerable<Summary> summaries, string path,
        ConcatReportImageOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndSaveAsImageAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsHtmlAsync(this IEnumerable<Summary> summaries, string path,
        ConcatReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndSaveAsHtmlAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and get HTML.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string ConcatReportsAndGetHtml(this IEnumerable<Summary> summaries, ConcatReportHtmlOptions options)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndGetHtml(options);
    }
    #endregion

    #region IEnumerable<BenchmarkInfo>
    /// <summary>
    /// Concatenates the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task ConcatReportsAndSaveAsHtmlAndImageAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string htmlPath, string imagePath,
        ConcatReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        await ConcatReportsAndSaveAsHtmlAsync(benchmarkInfo, htmlPath, options, cancellationToken);
        await ConcatReportsAndSaveAsImageAsync(benchmarkInfo, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and save as image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsImageAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        ConcatReportImageOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = ConcatReportsAndGetHtml(benchmarkInfo, ConcatReportHtmlOptions.From(options, HtmlDocumentWrapMode.Simple));
        return HtmlHelper.RenderHtmlToImageAsync(html, path, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsHtmlAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        ConcatReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = ConcatReportsAndGetHtml(benchmarkInfo, options);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and get HTML.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string ConcatReportsAndGetHtml(this IEnumerable<BenchmarkInfo> benchmarkInfo, ConcatReportHtmlOptions options)
    {
        var html = ConcatReportsAndGetHtmlCore(benchmarkInfo, options);
        return HtmlHelper.WrapInHtmlDocument(html, options.Title, options.HtmlWrapMode);
    }

    private static string ConcatReportsAndGetHtmlCore(this IEnumerable<BenchmarkInfo> benchmarkInfo, ConcatReportImageOptions options)
    {
        Guard.ThrowIfNullOrEmpty(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"<h1>{options.Title}</h1>");

        if (options.EnvironmentOnce)
            stringBuilder.AppendLine(HtmlHelper.WrapInCodeBlock(benchmarkInfo.First().EnvironmentInfo));

        foreach (var group in benchmarkInfo.GroupBy(p => p.GroupName))
        {
            var items = group.ToArray();

            stringBuilder.AppendLine($"<h2>{group.Key}</h2>");

            foreach (var item in items)
            {
                if (!(items.Length == 1 && group.Key == item.DisplayName))
                    stringBuilder.AppendLine($"<h3>{item.DisplayName}</h3>");

                if (options.EnvironmentOnce is false)
                    stringBuilder.AppendLine(HtmlHelper.WrapInCodeBlock(item.EnvironmentInfo));

                var result = item.Table.Process(options.GroupByColumns, options.SpectrumColumns, options.SortByColumns ?? options.SpectrumColumns, options.HighlightGroups, boldEntireRowOfLowestValue: true);
                var html = result.ToHtmlTable(options.DividerMode);
                stringBuilder.AppendLine(html);
            }
        }
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call

        return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }
    #endregion
    #endregion

    #region Markdown
    #region IEnumerable<Summary>
    /// <summary>
    /// Concatenates the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsMarkdownAsync(this IEnumerable<Summary> summaries, string path,
        ConcatReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndSaveAsMarkdownAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and get markdown.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string ConcatReportsAndGetMarkdown(this IEnumerable<Summary> summaries, ConcatReportMarkdownOptions options)
    {
        return summaries
            .GetBenchmarkInfo()
            .ConcatReportsAndGetMarkdown(options);
    }
    #endregion

    #region IEnumerable<BenchmarkInfo>
    /// <summary>
    /// Concatenates the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task ConcatReportsAndSaveAsMarkdownAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        ConcatReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var markdown = ConcatReportsAndGetMarkdown(benchmarkInfo, options);
        return File.WriteAllTextAsync(path, markdown, cancellationToken);
    }

    /// <summary>
    /// Concatenates the reports and get markdown.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string ConcatReportsAndGetMarkdown(this IEnumerable<BenchmarkInfo> benchmarkInfo, ConcatReportMarkdownOptions options)
    {
        Guard.ThrowIfNullOrEmpty(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();
#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"# {options.Title}");
        stringBuilder.AppendLine();

        if (options.EnvironmentOnce)
        {
            stringBuilder.AppendLine(MarkdownHelper.WrapInCodeBlock(benchmarkInfo.First().EnvironmentInfo));
            stringBuilder.AppendLine();
        }

        foreach (var group in benchmarkInfo.GroupBy(p => p.GroupName))
        {
            var items = group.ToArray();

            stringBuilder.AppendLine($"## {group.Key}");
            stringBuilder.AppendLine();

            foreach (var item in items)
            {
                if (!(items.Length == 1 && group.Key == item.DisplayName))
                {
                    stringBuilder.AppendLine($"### {item.DisplayName}");
                    stringBuilder.AppendLine();
                }

                if (options.EnvironmentOnce is false)
                {
                    stringBuilder.AppendLine(MarkdownHelper.WrapInCodeBlock(item.EnvironmentInfo));
                    stringBuilder.AppendLine();
                }

                var result = item.Table.Process(options.GroupByColumns, spectrumColumns: null, options.SortByColumns, highlightGroups: false, boldEntireRowOfLowestValue: true);
                var markdown = result.ToMarkdownTable(options.DividerMode);
                stringBuilder.AppendLine(markdown);
                stringBuilder.AppendLine();
            }
        }
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call

        return stringBuilder.ToString();
    }
    #endregion
    #endregion

#pragma warning restore S4136 // Method overloads should be grouped together
}