using BenchmarkDotNet.Reports;
using BenchmarkDotNetVisualizer.Utilities;
using System.Dynamic;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="BenchmarkInfo"/> class with holds information about a benchmark.
/// </summary>
/// <param name="summary">The summary.</param>
/// <param name="benchmarkType">Type of the benchmark.</param>
/// <param name="displayName">The display name.</param>
/// <param name="groupName">Name of the group.</param>
/// <param name="environmentInfo">The environment information.</param>
/// <param name="table">The table.</param>
public class BenchmarkInfo(Summary? summary, Type? benchmarkType, string displayName, string groupName, string environmentInfo, IEnumerable<ExpandoObject?> table)
{
    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    /// <value>
    /// The summary.
    /// </value>
    public Summary? Summary { get; set; } = summary;

    /// <summary>
    /// Gets or sets the benchmark type.
    /// </summary>
    /// <value>
    /// The type of the benchmark.
    /// </value>
    public Type? BenchmarkType { get; set; } = benchmarkType;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName { get; set; } = displayName;

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    /// <value>
    /// The name of the group.
    /// </value>
    public string GroupName { get; set; } = groupName;

    /// <summary>
    /// Gets or sets the environment information.
    /// </summary>
    /// <value>
    /// The environment information.
    /// </value>
    public string EnvironmentInfo { get; set; } = environmentInfo;

    /// <summary>
    /// Gets or sets the table.
    /// </summary>
    /// <value>
    /// The table.
    /// </value>
    public IEnumerable<ExpandoObject?> Table { get; set; } = table;

    /// <summary>
    /// Creates benchmark information from directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="benchmarksGroupName">Name of the benchmarks group.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <returns></returns>
    public static IEnumerable<BenchmarkInfo> CreateFromDirectory(string directory, string? benchmarksGroupName = null, string searchPattern = "*.md")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory, nameof(directory));
        ArgumentNullException.ThrowIfNull(searchPattern, nameof(searchPattern));

        var fileNames = Directory.GetFiles(directory, searchPattern);
        return CreateFromFile(fileNames, benchmarksGroupName);
    }

    /// <summary>
    /// Creates benchmark information from files.
    /// </summary>
    /// <param name="fileNames">The file names.</param>
    /// <param name="benchmarksGroupName">Name of the benchmarks group.</param>
    /// <returns></returns>
    public static IEnumerable<BenchmarkInfo> CreateFromFile(IEnumerable<string> fileNames, string? benchmarksGroupName = null)
    {
        Guard.ThrowIfNullOrEmpty(fileNames, nameof(fileNames));

        if (benchmarksGroupName is not null)
            return fileNames.Select(path => CreateFromFile(path, benchmarksGroupName));

        return fileNames.FindCommonPartOfEachFileNames().Select(p => CreateFromFile(p.Path, p.CommonPart.ExtractBenchmarkClassName()));
    }

    /// <summary>
    /// Creates benchmark information from file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="benchmarkGroupName">Name of the benchmark group.</param>
    /// <returns></returns>
    public static BenchmarkInfo CreateFromFile(string filePath, string? benchmarkGroupName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        var benchmarkClassName = filePath.ExtractBenchmarkClassName();
        benchmarkGroupName ??= benchmarkClassName;

#pragma warning disable S1117 // Local variables should not shadow class fields or properties
        var markdown = File.ReadAllText(filePath);
        var environmentInfo = BenchmarkVisualizer.ExtractEnvironmentInfo(markdown);
        var markdownTable = BenchmarkVisualizer.ExtractMarkdownTable(markdown);
        var table = MarkdownHelper.ParseMarkdownTable(markdownTable);
#pragma warning restore S1117 // Local variables should not shadow class fields or properties

        return new BenchmarkInfo(null, null, benchmarkClassName, benchmarkGroupName, environmentInfo, table);
    }

    /// <summary>
    /// Creates benchmark information from summary collection.
    /// </summary>
    /// <param name="summaries">The summaries.</param>
    /// <returns></returns>
    public static IEnumerable<BenchmarkInfo> CreateFromSummary(IEnumerable<Summary> summaries)
    {
        Guard.ThrowIfNullOrEmpty(summaries, nameof(summaries));

        return summaries.Select(summary => summary.GetBenchmarkInfo());
    }

    /// <summary>
    /// Creates benchmark information from summary.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    public static BenchmarkInfo CreateFromSummary(Summary summary)
    {
        ArgumentNullException.ThrowIfNull(summary, nameof(summary));

        var benchmarkClassType = summary.BenchmarksCases[0].Descriptor.WorkloadMethod.ReflectedType!;
        var benchmarkDisplayName = benchmarkClassType.GetDisplayName()!;
        var benchmarkGroupName = benchmarkClassType.GetGroupName() ?? benchmarkDisplayName;

#pragma warning disable S1117 // Local variables should not shadow class fields or properties
        var markdown = summary.GetExportedMarkdown();
        var environmentInfo = BenchmarkVisualizer.ExtractEnvironmentInfo(markdown);
        var markdownTable = BenchmarkVisualizer.ExtractMarkdownTable(markdown);
        var table = MarkdownHelper.ParseMarkdownTable(markdownTable);
#pragma warning restore S1117 // Local variables should not shadow class fields or properties

        return new BenchmarkInfo(summary, benchmarkClassType, benchmarkDisplayName, benchmarkGroupName, environmentInfo, table);
    }
}