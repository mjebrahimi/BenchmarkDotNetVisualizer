using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichHtmlExporterAttribute"/> class.
/// </summary>
/// <param name="title">The title.</param>
/// <param name="groupByColumns">The group by columns.</param>
/// <param name="sortByColumns">The sort by columns.</param>
/// <param name="spectrumColumns">The spectrum columns.</param>
/// <param name="highlightGroups">if set to <c>true</c> highlights groups.</param>
/// <param name="dividerMode">The divider mode. (Defaults to <see cref="RenderTableDividerMode.EmptyDividerRow"/>)</param>
/// <param name="htmlWrapMode">The HTML wrap mode. (Defaults to <see cref="HtmlDocumentWrapMode.Simple"/>)</param>
/// <param name="theme">The theme. (Defaults to <see cref="Theme.Dark"/>)</param>
/// <seealso cref="ExporterConfigBaseAttribute" />
public class RichHtmlExporterAttribute(
    string title,
    string[]? groupByColumns = null,
    string[]? sortByColumns = null,
    string[]? spectrumColumns = null,
    bool highlightGroups = true,
    RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow,
    HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple,
    Theme theme = Theme.Dark)
    : ExporterConfigBaseAttribute(new RichHtmlExporter(new()
    {
        Title = title,
        GroupByColumns = groupByColumns,
        SortByColumns = sortByColumns,
        SpectrumColumns = spectrumColumns,
        HighlightGroups = highlightGroups,
        DividerMode = dividerMode,
        HtmlWrapMode = htmlWrapMode,
        Theme = theme,
    }))
{
}