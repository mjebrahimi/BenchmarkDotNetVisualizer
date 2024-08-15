using BenchmarkDotNetVisualizer;

if (BenchmarkAutoRunner.IsRunningInDebugMode())
{
    Console.WriteLine("The IteratorsBenchmark needs to be run in RELEASE mode.");
    return;
}

var summary = BenchmarkAutoRunner.Run<IterationBenchmark>();

var options1 = new JoinReportHtmlOptions
{
    Title = "Performance Comparison between for, foreach, and ForEeach() method",
    MainColumn = "Method",
    GroupByColumns = ["Categories", "Length"],           //Groups by column 'Categories' and 'Length'
    PivotColumn = "Runtime",                             //Pivot 'Runtime' column per value of 'Mean'
    StatisticColumns = ["Mean"],                         //Colorizes 'Mean' columns as Spectrum
    ColumnsOrder = [".NET Core 3.0", ".NET Core 3.1", ".NET 5.0", ".NET 6.0", ".NET 7.0", ".NET 8.0"], //Order of columns
    DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
    HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    Theme = Theme.Dark                                   //Optional (Default is Dark)
};

await summary.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Runtime.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Runtime-Dark.png"),
    options: options1);

options1.Theme = Theme.Light;
await summary.JoinReportsAndSaveAsImageAsync(
    path: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Runtime-Light.png"),
    options: options1);

var options2 = new JoinReportHtmlOptions
{
    Title = "Performance Comparison between for, foreach, and ForEeach() method",
    MainColumn = "Runtime",
    GroupByColumns = ["Categories", "Length"],           //Groups by column 'Categories' and 'Length'
    PivotColumn = "Method",                              //Pivot 'Method' column per value of 'Mean'
    StatisticColumns = ["Mean"],                         //Colorizes 'Mean' columns as Spectrum
    ColumnsOrder = ["for", "foreach", "ForEach()"],      //Order of columns
    DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
    HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    Theme = Theme.Dark                                   //Optional (Default is Dark)
};
await summary.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Method.html"),
    imagePath: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Method-Dark.png"),
    options: options2);

options2.Theme = Theme.Light;
await summary.JoinReportsAndSaveAsImageAsync(
    path: DirectoryHelper.GetPathRelativeToProjectDirectory("Reports\\JoinedBenchmark-PivotBy-Method-Light.png"),
    options: options2);