using BenchmarkDotNetVisualizer.Utilities.Html;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Options for simple report and render as HTML output.
/// </summary>
/// <seealso cref="ReportImageOptions" />
public class ReportHtmlOptions : ReportImageOptions
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
    public static ReportHtmlOptions From(ReportImageOptions options, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        return new()
        {
            Title = options.Title,
            DividerMode = options.DividerMode,
            GroupByColumns = options.GroupByColumns,
            SpectrumColumns = options.SpectrumColumns,
            SortByColumns = options.SortByColumns,
            HighlightGroups = options.HighlightGroups,
            HtmlWrapMode = htmlWrapMode,
            ThemeOption = options.ThemeOption
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
/// Options for simple report and render as image.
/// </summary>
/// <seealso cref="ReportMarkdownOptions" />
public class ReportImageOptions : ReportMarkdownOptions
{
    /// <summary>
    /// Gets or sets the spectrum columns.
    /// </summary>
    /// <value>
    /// The spectrum columns.
    /// </value>
    public string[]? SpectrumColumns { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether highlight groups. (Defaults to <see langword="true"/>)
    /// </summary>
    /// <value>
    ///   <c>true</c> if highlight groups; otherwise, <c>false</c>.
    /// </value>
    public bool HighlightGroups { get; set; } = true;

    /// <summary>
    /// Specifies the theme for the image generation. default is Bright.
    /// </summary>
    public HtmlThemeOptions ThemeOption { get; set; } = HtmlThemeOptions.Bright;

    /// <inheritdoc />
    public override void Validate()
    {
        // Commented because of better developer experience
        //if (HighlightGroups && GroupByColumns.IsNullOrEmpty())
        //{
        //    throw new InvalidDataException(
        //        $"Argument '{nameof(HighlightGroups)}' is set to true but '{nameof(GroupByColumns)}' are not specified." +
        //        $" Set '{nameof(HighlightGroups)}' to false or provide '{nameof(GroupByColumns)}'.");
        //}

        base.Validate();
    }
}

/// <summary>
/// Options for simple report and render as markdown output.
/// </summary>
public class ReportMarkdownOptions
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public required string Title { get; set; }
#else
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; } = null!;
#endif

    /// <summary>
    /// Gets or sets the group by columns.
    /// </summary>
    /// <value>
    /// The group by columns.
    /// </value>
    public string[]? GroupByColumns { get; set; }

    /// <summary>
    /// Gets or sets the sort by columns.
    /// </summary>
    /// <value>
    /// The sort by columns.
    /// </value>
    public string[]? SortByColumns { get; set; }

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
    }
}