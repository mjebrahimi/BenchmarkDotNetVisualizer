using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichMarkdownExporter"/> class.
/// </summary>
/// <param name="options">The options.</param>
/// <seealso cref="ExporterBase" />
public class RichMarkdownExporter(ReportMarkdownOptions options) : ExporterBase
{
    /// <inheritdoc/>
    protected override string FileExtension => "md";

    /// <inheritdoc/>
    protected override string FileNameSuffix => "-rich";

    /// <inheritdoc/>
    public override void ExportToLog(Summary summary, ILogger logger)
    {
        if (summary.Table.FullContent.Length == 0)
        {
            logger.WriteLineError("There are no benchmarks found ");
            logger.WriteLine();
            return;
        }
        var markdown = summary.GetMarkdown(options);
        logger.Write(markdown);
    }
}