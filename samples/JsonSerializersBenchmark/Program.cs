using BenchmarkDotNetVisualizer;

var summary = BenchmarkAutoRunner.Run<JsonSerializersBenchmark>();

var options = new ReportHtmlOptions
{
    Title = "Json Serializers Benchmark",
    GroupByColumns = ["Method"],                          //Colorizes 'Mean' and 'Allocated' columns as Spectrum
    SpectrumColumns = ["Mean", "Allocated"],              //Colorizes 'Mean' columns as Spectrum
    DividerMode = RenderTableDividerMode.EmptyDividerRow, //Separates tables by Empty Divider Row
    HtmlWrapMode = HtmlDocumentWrapMode.Simple,
    Theme = Theme.Dark                                    //Optional (Default is Dark)
};

await summary.SaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark-Dark.png"),
    options: options);

options.Theme = Theme.Light;
await summary.SaveAsImageAsync(
    path: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Benchmark-Light.png"),
    options: options);