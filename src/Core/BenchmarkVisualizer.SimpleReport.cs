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
    /// Saves report as HTML and image asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsHtmlAndImageAsync(this Summary summary, string htmlPath, string imagePath,
        ReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .SaveAsHtmlAndImageAsync(htmlPath, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Saves report as image asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsImageAsync(this Summary summary, string path,
        ReportImageOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .SaveAsImageAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Saves report as HTML asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsHtmlAsync(this Summary summary, string path,
        ReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .SaveAsHtmlAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Gets the HTML report.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string GetHtml(this Summary summary, ReportHtmlOptions options)
    {
        return summary
            .GetBenchmarkInfo()
            .GetHtml(options);
    }
    #endregion

    #region BenchmarkInfo
    /// <summary>
    /// Saves report as HTML and image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="htmlPath">The HTML path.</param>
    /// <param name="imagePath">The image path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SaveAsHtmlAndImageAsync(this BenchmarkInfo benchmarkInfo, string htmlPath, string imagePath,
        ReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        await SaveAsHtmlAsync(benchmarkInfo, htmlPath, options, cancellationToken);
        await SaveAsImageAsync(benchmarkInfo, imagePath, options, cancellationToken);
    }

    /// <summary>
    /// Saves report as image asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsImageAsync(this BenchmarkInfo benchmarkInfo, string path,
        ReportImageOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = GetHtml(benchmarkInfo, ReportHtmlOptions.From(options, HtmlDocumentWrapMode.Simple));
        return HtmlHelper.RenderHtmlToImageAsync(html, path, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Saves report as HTML asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsHtmlAsync(this BenchmarkInfo benchmarkInfo, string path,
        ReportHtmlOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var html = GetHtml(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Gets the HTML report.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string GetHtml(this BenchmarkInfo benchmarkInfo, ReportHtmlOptions options)
    {
        var html = GetHtmlCore(benchmarkInfo, options);
        return HtmlHelper.WrapInHtmlDocument(html, options.Title,options.ThemeOption, options.HtmlWrapMode);
    }

    private static string GetHtmlCore(this BenchmarkInfo benchmarkInfo, ReportImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"<h1>{options.Title}</h1>");

        stringBuilder.AppendLine(HtmlHelper.WrapInCodeBlock(benchmarkInfo.EnvironmentInfo));

        stringBuilder.AppendLine($"<h2>{benchmarkInfo.DisplayName}</h2>");

        var result = benchmarkInfo.Table.Process(options.GroupByColumns, options.SpectrumColumns, options.SortByColumns ?? options.SpectrumColumns, options.HighlightGroups, boldEntireRowOfLowestValue: true);
        var html = result.ToHtmlTable(options.DividerMode);

        stringBuilder.AppendLine(html);
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call

        return stringBuilder.ToString();
    }
    #endregion
    #endregion

    #region Markdown
    #region Summary
    /// <summary>
    /// Saves report as markdown asynchronously.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsMarkdownAsync(this Summary summary, string path, ReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        return summary
            .GetBenchmarkInfo()
            .SaveAsMarkdownAsync(path, options, cancellationToken);
    }

    /// <summary>
    /// Gets the markdown report.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string GetMarkdown(this Summary summary, ReportMarkdownOptions options)
    {
        return summary
            .GetBenchmarkInfo()
            .GetMarkdown(options);
    }
    #endregion

    #region BenchmarkInfo
    /// <summary>
    /// Saves report as markdown asynchronously.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="path">The path.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsMarkdownAsync(this BenchmarkInfo benchmarkInfo, string path, ReportMarkdownOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var markdown = GetMarkdown(benchmarkInfo, options);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, markdown, cancellationToken);
    }

    /// <summary>
    /// Gets the markdown report.
    /// </summary>
    /// <param name="benchmarkInfo">The benchmark information.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static string GetMarkdown(this BenchmarkInfo benchmarkInfo, ReportMarkdownOptions options)
    {
        ArgumentNullException.ThrowIfNull(benchmarkInfo, nameof(benchmarkInfo));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        options.Validate();

        var stringBuilder = new StringBuilder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
        stringBuilder.AppendLine($"# {options.Title}");
        stringBuilder.AppendLine();

        stringBuilder.AppendLine(MarkdownHelper.WrapInCodeBlock(benchmarkInfo.EnvironmentInfo));
        stringBuilder.AppendLine();

        stringBuilder.AppendLine($"## {benchmarkInfo.DisplayName}");
        stringBuilder.AppendLine();

        var result = benchmarkInfo.Table.Process(options.GroupByColumns, spectrumColumns: null, options.SortByColumns, highlightGroups: false, boldEntireRowOfLowestValue: true);
        var markdown = result.ToMarkdownTable(options.DividerMode);

        stringBuilder.AppendLine(markdown);
        stringBuilder.AppendLine();
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call

        return stringBuilder.ToString();
    }
    #endregion
    #endregion

#pragma warning restore S4136 // Method overloads should be grouped together
}