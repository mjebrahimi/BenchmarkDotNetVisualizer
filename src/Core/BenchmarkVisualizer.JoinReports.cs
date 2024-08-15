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
    #region Summary
    /// <summary>
    /// Joins the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAndImageAsync(this Summary summary, string htmlPath, string imagePath,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsHtmlAndImageAsync(htmlPath, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as image asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsImageAsync(this Summary summary, string path,
        JoinReportImageOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsImageAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAsync(this Summary summary, string path,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsHtmlAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get HTML.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetHtml(this Summary summary, JoinReportHtmlOptions options)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndGetHtml(options);
    }
    #endregion

    #region BenchmarkInfo
    /// <summary>
    /// Joins the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task JoinReportsAndSaveAsHtmlAndImageAsync(this BenchmarkInfo benchmarkInfo, string htmlPath, string imagePath,
    JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        await JoinReportsAndSaveAsHtmlAsync(benchmarkInfo, htmlPath, options, cancellationToken);
        await JoinReportsAndSaveAsImageAsync(benchmarkInfo, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsImageAsync(this BenchmarkInfo benchmarkInfo, string path,
        JoinReportImageOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = JoinReportsAndGetHtml(benchmarkInfo, JoinReportHtmlOptions.From(options, HtmlDocumentWrapMode.Simple));
        return HtmlHelper.RenderHtmlToImageAsync(html, path, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAsync(this BenchmarkInfo benchmarkInfo, string path,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = JoinReportsAndGetHtml(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get HTML.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetHtml(this BenchmarkInfo benchmarkInfo, JoinReportHtmlOptions options)
    {
        var html = JoinReportsAndGetHtmlCore([benchmarkInfo], options);
        return HtmlHelper.WrapInHtmlDocument(html, options.Title, options.Theme, options.HtmlWrapMode);
    }
    #endregion

    #region IEnumerable<Summary>
    /// <summary>
    /// Joins the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAndImageAsync(this IEnumerable<Summary> summaries, string htmlPath, string imagePath,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsHtmlAndImageAsync(htmlPath, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as image asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsImageAsync(this IEnumerable<Summary> summaries, string path,
        JoinReportImageOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsImageAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAsync(this IEnumerable<Summary> summaries, string path,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsHtmlAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get HTML.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetHtml(this IEnumerable<Summary> summaries, JoinReportHtmlOptions options)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndGetHtml(options);
    }
    #endregion

    #region IEnumerable<BenchmarkInfo>
    /// <summary>
    /// Joins the reports and save as HTML and image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task JoinReportsAndSaveAsHtmlAndImageAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string htmlPath, string imagePath,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        await JoinReportsAndSaveAsHtmlAsync(benchmarkInfo, htmlPath, options, cancellationToken);
        await JoinReportsAndSaveAsImageAsync(benchmarkInfo, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsImageAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        JoinReportImageOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = JoinReportsAndGetHtml(benchmarkInfo, JoinReportHtmlOptions.From(options, HtmlDocumentWrapMode.Simple));
        return HtmlHelper.RenderHtmlToImageAsync(html, path, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Joins the reports and save as HTML asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsHtmlAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        JoinReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = JoinReportsAndGetHtml(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get HTML.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetHtml(this IEnumerable<BenchmarkInfo> benchmarkInfo, JoinReportHtmlOptions options)
    {
        var html = JoinReportsAndGetHtmlCore(benchmarkInfo, options);
        return HtmlHelper.WrapInHtmlDocument(html, options.Title, options.Theme, options.HtmlWrapMode);
    }

    private static string JoinReportsAndGetHtmlCore(this IEnumerable<BenchmarkInfo> benchmarkInfo, JoinReportImageOptions options)
    {
        Guard.ThrowIfNullOrEmpty(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"<h1>{options.Title}</h1>");
        stringBuilder.AppendLine(HtmlHelper.WrapInCodeBlock(benchmarkInfo.First().EnvironmentInfo));

        foreach (var group in benchmarkInfo.GroupBy(p => p.GroupName))
        {
            var h2Title = options.StatisticColumns.Length == 1 ? $"Comparison: {options.StatisticColumns[0]}" : group.Key;
            stringBuilder.AppendLine($"<h2>{h2Title}</h2>");

            var tables = group.Select(p => p.Table);

            foreach (var statisticColumn in options.StatisticColumns)
            {
                var result = tables.JoinAndProcess(options.MainColumn, options.GroupByColumns, options.PivotColumn, statisticColumn,
                    options.ColumnsOrder, options.OtherColumnsToSelect, options.SpectrumStatisticColumn, options.HighlightGroups);

                var html = result.ToHtmlTable(options.DividerMode);

                if (options.StatisticColumns.Length > 1)
                    stringBuilder.AppendLine($"<h3>Comparison: {statisticColumn}</h3>");
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
    /// Joins the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsMarkdownAsync(this Summary summary, string path,
        JoinReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsMarkdownAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get markdown.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetMarkdown(this Summary summary, JoinReportMarkdownOptions options)
    {
        return summary
            .GetBenchmarkInfo()
            .JoinReportsAndGetMarkdown(options);
    }
    #endregion

    #region BenchmarkInfo
    /// <summary>
    /// Joins the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsMarkdownAsync(this BenchmarkInfo benchmarkInfo, string path,
        JoinReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var markdown = JoinReportsAndGetMarkdown(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, markdown, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get markdown.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetMarkdown(this BenchmarkInfo benchmarkInfo, JoinReportMarkdownOptions options)
    {
        return JoinReportsAndGetMarkdown([benchmarkInfo], options);
    }
    #endregion

    #region IEnumerable<Summary>
    /// <summary>
    /// Joins the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsMarkdownAsync(this IEnumerable<Summary> summaries, string path,
        JoinReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndSaveAsMarkdownAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get markdown.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetMarkdown(this IEnumerable<Summary> summaries, JoinReportMarkdownOptions options)
    {
        return summaries
            .GetBenchmarkInfo()
            .JoinReportsAndGetMarkdown(options);
    }
    #endregion

    #region IEnumerable<BenchmarkInfo>
    /// <summary>
    /// Joins the reports and save as markdown asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task JoinReportsAndSaveAsMarkdownAsync(this IEnumerable<BenchmarkInfo> benchmarkInfo, string path,
        JoinReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var markdown = JoinReportsAndGetMarkdown(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, markdown, cancellationToken);
    }

    /// <summary>
    /// Joins the reports and get markdown.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string JoinReportsAndGetMarkdown(this IEnumerable<BenchmarkInfo> benchmarkInfo, JoinReportMarkdownOptions options)
    {
        Guard.ThrowIfNullOrEmpty(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"# {options.Title}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(MarkdownHelper.WrapInCodeBlock(benchmarkInfo.First().EnvironmentInfo));
        stringBuilder.AppendLine();

        foreach (var group in benchmarkInfo.GroupBy(p => p.GroupName))
        {
            var h2Title = options.StatisticColumns.Length == 1 ? $"Comparison: {options.StatisticColumns[0]}" : group.Key;
            stringBuilder.AppendLine($"## {h2Title}");
            stringBuilder.AppendLine();

            var tables = group.Select(p => p.Table);

            foreach (var statisticColumn in options.StatisticColumns)
            {
                var result = tables.JoinAndProcess(options.MainColumn, options.GroupByColumns, options.PivotColumn, statisticColumn,
                    options.ColumnsOrder, options.OtherColumnsToSelect, spectrumStatisticColumn: false, highlightGroups: false);

                var markdown = result.ToMarkdownTable(options.DividerMode);

                if (options.StatisticColumns.Length > 1)
                {
                    stringBuilder.AppendLine($"### Comparison: {statisticColumn}");
                    stringBuilder.AppendLine();
                }
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