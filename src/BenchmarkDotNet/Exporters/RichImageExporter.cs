using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNetVisualizer;

/// <summary>
/// Initializes a new instance of the <see cref="RichImageExporter"/> class.
/// </summary>
/// <param name="options">The options.</param>
/// <param name="format">The image format. (Defaults to <see cref="ImageFormat.Png"/>)</param>
/// <seealso cref="ExporterBase" />
public class RichImageExporter(ReportImageOptions options, ImageFormat format = ImageFormat.Png) : IExporter
{
    /// <inheritdoc/>
    public string Name => GetType().Name + FileNameSuffix;

    /// <inheritdoc/>
    public string FileExtension => format.GetExtension();

    /// <inheritdoc/>
    public string FileCaption => "report";

    /// <inheritdoc/>
    public string FileNameSuffix => "-rich";

    /// <inheritdoc/>
    public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
    {
        string fileName = GetFileName(summary);
        string text = GetArtifactFullName(summary);
        if (File.Exists(text))
        {
            try
            {
                File.Delete(text);
            }
            catch (IOException)
            {
                string text2 = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string text3 = Path.Combine(summary.ResultsDirectoryPath, fileName) + "-" + FileCaption + FileNameSuffix + "-" + text2 + "." + FileExtension;
                consoleLogger.WriteLineError("Could not overwrite file " + text + ". Exporting to " + text3);
                text = text3;
            }
        }

        ExportToLog(summary, null!);

        return new string[1] { text };
    }

    /// <inheritdoc/>
    public void ExportToLog(Summary summary, ILogger logger)
    {
        var path = GetArtifactFullName(summary);
        summary.SaveAsImageAsync(path, options).GetAwaiter().GetResult(); //.RunSynchronously();
    }

    /// <summary>
    /// Gets the full name of the artifact.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    internal string GetArtifactFullName(Summary summary)
    {
        string fileName = GetFileName(summary);
        return $"{Path.Combine(summary.ResultsDirectoryPath, fileName)}-{FileCaption}{FileNameSuffix}.{FileExtension}";
    }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <param name="summary">The summary.</param>
    /// <returns></returns>
    private static string GetFileName(Summary summary)
    {
        Type[] array = summary.BenchmarksCases.Select((BenchmarkCase b) => b.Descriptor.Type).Distinct().ToArray();
        if (array.Length == 1)
        {
            return FolderNameHelper.ToFolderName(array.Single());
        }

        return summary.Title;
    }
}