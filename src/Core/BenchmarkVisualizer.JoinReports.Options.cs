using BenchmarkDotNetVisualizer.Utilities;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Options for join reports and render as HTML output.
/// </summary>
/// <seealso cref="JoinReportImageOptions" />
public class JoinReportHtmlOptions : JoinReportImageOptions
{
    /// <summary>
    /// Gets or sets the HTML wrap mode. (Defaults to <see cref="HtmlDocumentWrapMode.Simple"/>)
    /// </summary>
    /// <value>
    /// The HTML wrap mode.
    /// </value>
    public HtmlDocumentWrapMode HtmlWrapMode { get; set; } = HtmlDocumentWrapMode.Simple;

    /// <summary>
    /// Creates from the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <returns></returns>
    public static JoinReportHtmlOptions From(JoinReportImageOptions options, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        return new()
        {
            Title = options.Title,
            MainColumn = options.MainColumn,
            GroupByColumns = options.GroupByColumns,
            PivotColumn = options.PivotColumn,
            StatisticColumns = options.StatisticColumns,
            OtherColumnsToSelect = options.OtherColumnsToSelect,
            ColumnsOrder = options.ColumnsOrder,
            DividerMode = options.DividerMode,
            SpectrumStatisticColumn = options.SpectrumStatisticColumn,
            HighlightGroups = options.HighlightGroups,
            Theme = options.Theme,
            HtmlWrapMode = htmlWrapMode,
        };
    }

    /// <inheritdoc />
    public override void Validate()
    {
        if (HtmlWrapMode == HtmlDocumentWrapMode.RichDataTables && DividerMode == RenderTableDividerMode.EmptyDividerRow)
            throw new InvalidOperationException($"{nameof(HtmlWrapMode)}({HtmlWrapMode}) and {nameof(DividerMode)} ({DividerMode}) aren't compatible with each other.");

        base.Validate();
    }
}

/// <summary>
/// Options for join reports and render as image.
/// </summary>
/// <seealso cref="JoinReportMarkdownOptions" />
public class JoinReportImageOptions : JoinReportMarkdownOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether spectrum statistic column. (Defaults to <see langword="true"/>)
    /// </summary>
    /// <value>
    ///   <c>true</c> if spectrum statistic column; otherwise, <c>false</c>.
    /// </value>
    public bool SpectrumStatisticColumn { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether highlight groups. (Defaults to <see langword="true"/>)
    /// </summary>
    /// <value>
    ///   <c>true</c> if highlight groups; otherwise, <c>false</c>.
    /// </value>
    public bool HighlightGroups { get; set; } = true;

    /// <summary>
    /// Gets or sets the theme of the report. (Defaults to <see cref="Theme.Dark"/>)
    /// </summary>
    public Theme Theme { get; set; } = Theme.Dark;
}

/// <summary>
/// Options for join reports and render as markdown output.
/// </summary>
public class JoinReportMarkdownOptions
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the main column.
    /// </summary>
    /// <value>
    /// The main column.
    /// </value>
    public required string MainColumn { get; set; }

    /// <summary>
    /// Gets or sets the group by columns.
    /// </summary>
    /// <value>
    /// The group by columns.
    /// </value>
    public required string[] GroupByColumns { get; set; }

    /// <summary>
    /// Gets or sets the pivot property.
    /// </summary>
    /// <value>
    /// The pivot property.
    /// </value>
    public required string PivotColumn { get; set; }

    /// <summary>
    /// Gets or sets the statistic columns.
    /// </summary>
    /// <value>
    /// The statistic columns.
    /// </value>
    public required string[] StatisticColumns { get; set; }

    /// <summary>
    /// Gets or sets the columns order.
    /// </summary>
    /// <value>
    /// The order columns.
    /// </value>
    public required string[] ColumnsOrder { get; set; }
#else
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the main column.
    /// </summary>
    /// <value>
    /// The main column.
    /// </value>
    public string MainColumn { get; set; } = null!;

    /// <summary>
    /// Gets or sets the group by columns.
    /// </summary>
    /// <value>
    /// The group by columns.
    /// </value>
    public string[] GroupByColumns { get; set; } = null!;

    /// <summary>
    /// Gets or sets the pivot property.
    /// </summary>
    /// <value>
    /// The pivot property.
    /// </value>
    public string PivotColumn { get; set; } = null!;

    /// <summary>
    /// Gets or sets the statistic columns.
    /// </summary>
    /// <value>
    /// The statistic columns.
    /// </value>
    public string[] StatisticColumns { get; set; } = null!;

    /// <summary>
    /// Gets or sets the columns order.
    /// </summary>
    /// <value>
    /// The order columns.
    /// </value>
    public string[] ColumnsOrder { get; set; } = null!;
#endif

    /// <summary>
    /// Gets or sets the other columns to select.
    /// </summary>
    /// <value>
    /// The other columns to select.
    /// </value>
    public string[]? OtherColumnsToSelect { get; set; }

    /// <summary>
    /// Gets or sets the divider mode. (Defaults to <see cref="RenderTableDividerMode.EmptyDividerRow"/>)
    /// </summary>
    /// <value>
    /// The divider mode.
    /// </value>
    public virtual RenderTableDividerMode DividerMode { get; set; } = RenderTableDividerMode.EmptyDividerRow;

    /// <summary>
    /// Validates this instance and throws exception if it's not valid.
    /// </summary>
    public virtual void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Title, nameof(Title));
        Guard.ThrowIfNullOrEmpty(GroupByColumns, nameof(GroupByColumns));
        ArgumentException.ThrowIfNullOrWhiteSpace(PivotColumn, nameof(PivotColumn));
        Guard.ThrowIfNullOrEmpty(StatisticColumns, nameof(StatisticColumns));
        Guard.ThrowIfNullOrEmpty(ColumnsOrder, nameof(ColumnsOrder));
    }
}