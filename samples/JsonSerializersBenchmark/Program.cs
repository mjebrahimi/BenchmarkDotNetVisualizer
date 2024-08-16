using BenchmarkDotNetVisualizer;

var summary = BenchmarkAutoRunner.Run<JsonSerializersBenchmark>();

var options = new ReportHtmlOptions
{
    Title = "Json Serializers Benchmark",
    GroupByColumns = ["Method"],                          // Groups by 'Method' column and highlights groups
    SpectrumColumns = ["Mean", "Allocated"],              // Colorizes 'Mean' and 'Allocated' columns as Spectrum
    DividerMode = RenderTableDividerMode.EmptyDividerRow, // Separates tables by Empty Divider Row
    HtmlWrapMode = HtmlDocumentWrapMode.Simple,           // Uses simple HTML table
    Theme = Theme.Dark                                    // Optional (Default is Dark)
};

await summary.SaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark-Dark.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark-Dark.png"),
    options: options);

options.Theme = Theme.Light;
await summary.SaveAsImageAsync(
    path: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark-Light.png"),
    options: options);