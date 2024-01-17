using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNetVisualizer.Utilities;
using System.Dynamic;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Benchmark Visualizer
/// </summary>
public static partial class BenchmarkVisualizer
{
    #region Benchmark Summary Extensions
    /// <summary>
    /// Gets the benchmark information.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <returns></returns>
    public static IEnumerable<BenchmarkInfo> GetBenchmarkInfo(this IEnumerable<Summary> summaries)
    {
        return BenchmarkInfo.CreateFromSummary(summaries);
    }

    /// <summary>
    /// Gets the benchmark information.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    public static BenchmarkInfo GetBenchmarkInfo(this Summary summary)
    {
        return BenchmarkInfo.CreateFromSummary(summary);
    }

    /// <summary>
    /// Gets the report table.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <param name="rowDivider">The row divider.</param>
    /// <returns></returns>
    public static IEnumerable<ExpandoObject?> GetReportTable(this Summary summary, ParseTableDividerMode rowDivider = ParseTableDividerMode.PlaceNull)
    {
        ArgumentNullException.ThrowIfNull(summary, nameof(summary));

        var markdown = summary.GetMarkdownTable();
        return MarkdownHelper.ParseMarkdownTable(markdown, rowDivider);
    }

    /// <summary>
    /// Gets the environment information from benchmark <paramref name="summary"/>.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    public static string GetEnvironmentInfo(this Summary summary)
    {
        ArgumentNullException.ThrowIfNull(summary, nameof(summary));

        var markdown = summary.GetExportedMarkdown();
        return ExtractEnvironmentInfo(markdown);
    }

    /// <summary>
    /// Gets the markdown table from benchmark <paramref name="summary"/>.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    public static string GetMarkdownTable(this Summary summary)
    {
        ArgumentNullException.ThrowIfNull(summary, nameof(summary));

        var markdown = summary.GetExportedMarkdown();
        return ExtractMarkdownTable(markdown);
    }
    #endregion

    #region ExtractEnvironmentInfo/TableInfo
    /// <summary>
    /// Extracts the environment information from markdown text of a benchmark.
    /// </summary>
    /// <param name="markdown">The markdown.</param>
    /// <returns></returns>
    public static string ExtractEnvironmentInfo(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown, nameof(markdown));
        ArgumentException.ThrowIfNullOrEmpty(markdown, nameof(markdown));

#pragma warning disable S3878 // Arrays should not be created for params parameters
        return markdown[..(markdown.LastIndexOf("```") + 4)].Trim([.. Environment.NewLine.ToCharArray(), ' ', '`']);
#pragma warning restore S3878 // Arrays should not be created for params parameters
    }

    /// <summary>
    /// Extracts the markdown table from markdown text of a benchmark.
    /// </summary>
    /// <param name="markdown">The markdown.</param>
    /// <returns></returns>
    public static string ExtractMarkdownTable(string markdown)
    {
        ArgumentException.ThrowIfNullOrEmpty(markdown, nameof(markdown));

#pragma warning disable S3878 // Arrays should not be created for params parameters
        return markdown[(markdown.LastIndexOf("```") + 4)..].Trim([.. Environment.NewLine.ToCharArray(), ' ']);
#pragma warning restore S3878 // Arrays should not be created for params parameters
    }
    #endregion

    #region private methods
    /// <summary>
    /// Gets the markdown text from benchmark <paramref name="summary"/> using <see cref="MarkdownExporter.GitHub"/> exporter.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    internal static string GetExportedMarkdown(this Summary summary)
    {
        ArgumentNullException.ThrowIfNull(summary, nameof(summary));

        using var writer = new StringWriter();
        using var logger = new TextLogger(writer);
        MarkdownExporter.GitHub.ExportToLog(summary, logger);
        return writer.ToString();
    }

    /// <summary>
    /// Finds the common part of each file names.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns></returns>
    internal static (string Path, string CommonPart)[] FindCommonPartOfEachFileNames(this IEnumerable<string> paths)
    {
        Guard.ThrowIfNullOrEmpty(paths, nameof(paths));

        var array = paths
            .Select(path =>
            {
                var fileName = Path.GetFileName(path);
                return new { Path = path, FileName = fileName };
            }).ToArray();

        return array
            .Select(item =>
            {
                var commonParts = array.Select(other => FindCommonPart(item.FileName, other.FileName)).ToArray();
                var bestCommonPart = commonParts.Where(p => p.Length != 0).MinBy(p => p.Length)!.Trim('_', '-');
                return (item.Path, bestCommonPart);
            }).ToArray();
    }

    /// <summary>
    /// Finds the starting common part of many strings.
    /// </summary>
    /// <param name="strings">The strings.</param>
    /// <returns></returns>
    internal static string FindCommonPart(params string[] strings)
    {
        return strings.Aggregate((current, next) => string.Concat(current.TakeWhile((c, i) => c == next[i])));
    }

    /// <summary>
    /// Extracts the name of the benchmark class from fileName (filePath).
    /// </summary>
    /// <param name="fileName">The filename.</param>
    /// <returns></returns>
    internal static string ExtractBenchmarkClassName(this string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName, nameof(fileName));

        fileName = Path.GetFileName(fileName);

        var index = fileName.LastIndexOf("-report");
        if (index > 0)
            fileName = fileName[..index];

        return fileName.Trim('_', '-');
    }
    #endregion
}