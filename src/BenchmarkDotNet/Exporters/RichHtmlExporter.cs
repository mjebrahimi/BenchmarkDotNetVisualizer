using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichHtmlExporter"/> class.
/// </summary>
/// <param name="options">The options.</param>
/// <seealso cref="ExporterBase" />
public class RichHtmlExporter(ReportHtmlOptions options) : ExporterBase
{
    /// <inheritdoc/>
    protected override string FileExtension => "html";

    /// <inheritdoc/>
    protected override string FileNameSuffix => "-rich";

    /// <inheritdoc/>
    public override void ExportToLog(Summary summary, ILogger logger)
    {
        if (summary.Table.FullContent.Length == 0)
        {
            logger.WriteLineError("<pre>There are no benchmarks found</pre>");
            return;
        }
        var html = summary.GetHtml(options);
        logger.Write(html);
    }
}