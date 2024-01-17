using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichImageExporterAttribute"/> class.
/// </summary>
/// <param name="title">The title.</param>
/// <param name="groupByColumns">The group by columns.</param>
/// <param name="sortByColumns">The sort by columns.</param>
/// <param name="spectrumColumns">The spectrum columns.</param>
/// <param name="highlightGroups">if set to <c>true</c> highlights groups.</param>
/// <param name="dividerMode">The divider mode.</param>
/// <param name="format">The image format. (Defaults to <see cref="ImageFormat.Png"/>)</param>
/// <seealso cref="ExporterConfigBaseAttribute" />
public class RichImageExporterAttribute(
    string title,
    string[]? groupByColumns = null,
    string[]? sortByColumns = null,
    string[]? spectrumColumns = null,
    bool highlightGroups = true,
    RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow,
    ImageFormat format = ImageFormat.Png)
    : ExporterConfigBaseAttribute(new RichImageExporter(new()
    {
        Title = title,
        GroupByColumns = groupByColumns,
        SortByColumns = sortByColumns,
        SpectrumColumns = spectrumColumns,
        HighlightGroups = highlightGroups,
        DividerMode = dividerMode
    }, format))
{
}