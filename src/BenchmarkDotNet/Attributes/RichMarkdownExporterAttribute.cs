using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichMarkdownExporterAttribute"/> class.
/// </summary>
/// <param name="title">The title.</param>
/// <param name="groupByColumns">The group by columns.</param>
/// <param name="sortByColumns">The sort by columns.</param>
/// <param name="dividerMode">The divider mode.</param>
/// <seealso cref="ExporterConfigBaseAttribute" />
public class RichMarkdownExporterAttribute(
    string title,
    string[]? groupByColumns = null,
    string[]? sortByColumns = null,
    RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    : ExporterConfigBaseAttribute(new RichMarkdownExporter(new()
    {
        Title = title,
        GroupByColumns = groupByColumns,
        SortByColumns = sortByColumns,
        DividerMode = dividerMode,
    }))
{
}