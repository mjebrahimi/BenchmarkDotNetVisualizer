using BenchmarkDotNetVisualizer;

#region JsonSerializersBenchmark
#if NET6_0_OR_GREATER
var summary1 = BenchmarkAutoRunner.Run<JsonSerializersBenchmark>();

await summary1.SaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JsonSerializers\\Benchmark.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JsonSerializers\\Benchmark.png"),
    options: new ReportHtmlOptions
    {
        Title = "Json Serializers Benchmark",
        GroupByColumns = ["Method"],                          // Colorizes 'Mean' and 'Allocated' columns as Spectrum
        SpectrumColumns = ["Mean", "Allocated"],
        DividerMode = RenderTableDividerMode.EmptyDividerRow, // Separates tables by Empty Divider Row
        HtmlWrapMode = HtmlDocumentWrapMode.Simple,
    });
#endif
#endregion

#region IteratorsBenchmark
#if RELEASE
var summary2 = BenchmarkAutoRunner.Run<IteratorsBenchmark>();

await summary2.SaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\Benchmark.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\Benchmark.png"),
    options: new ReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEeach() method",
        GroupByColumns = ["Runtime"],
        SpectrumColumns = ["Mean", "Allocated"],             // Colorizes 'Mean' and 'Allocated' columns as Spectrum
        DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    });

await summary2.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\JoinedBenchmark-PivotBy-Runtime.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\JoinedBenchmark-PivotBy-Runtime.png"),
    options: new JoinReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEeach() method",
        MainColumn = "Method",
        GroupByColumns = ["Categories", "Length"],           // Groups by column 'Categories' and 'Length'
        PivotProperty = "Runtime",
        StatisticColumns = ["Mean"],
        ColumnsOrder = [".NET Core 3.0", ".NET Core 3.1", ".NET 5.0", ".NET 6.0", ".NET 7.0", ".NET 8.0"], // Order of columns
        DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    });

await summary2.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\JoinedBenchmark-PivotBy-Method.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\Iterators\\JoinedBenchmark-PivotBy-Method.png"),
    options: new JoinReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEeach() method",
        MainColumn = "Runtime",
        GroupByColumns = ["Categories", "Length"],           // Groups by column 'Categories' and 'Length'
        PivotProperty = "Method",
        StatisticColumns = ["Mean"],
        ColumnsOrder = ["for", "foreach", "ForEach()"],
        DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    });
#endif
#endregion