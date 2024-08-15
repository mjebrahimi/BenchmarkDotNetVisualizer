namespace BenchmarkDotNetVisualizer.Tests;

public class IterationBenchmarkTests : TestBase
{
    [Fact]
    public async Task Joined_PivotBy_Runtime_Dark()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "IterationBenchmark-report-github.md").First();

        var options = new JoinReportHtmlOptions
        {
            Title = "Performance Comparison between for, foreach, and ForEeach() method",
            MainColumn = "Method",
            GroupByColumns = ["Categories", "Length"],
            PivotColumn = "Runtime",
            StatisticColumns = ["Mean"],
            ColumnsOrder = [".NET Core 3.0", ".NET Core 3.1", ".NET 5.0", ".NET 6.0", ".NET 7.0", ".NET 8.0"],
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, imgPath) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }

    [Fact]
    public async Task Joined_PivotBy_Runtime_Light()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "IterationBenchmark-report-github.md").First();

        var options = new JoinReportHtmlOptions
        {
            Title = "Performance Comparison between for, foreach, and ForEeach() method",
            MainColumn = "Method",
            GroupByColumns = ["Categories", "Length"],
            PivotColumn = "Runtime",
            StatisticColumns = ["Mean"],
            ColumnsOrder = [".NET Core 3.0", ".NET Core 3.1", ".NET 5.0", ".NET 6.0", ".NET 7.0", ".NET 8.0"],
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Light
        };

        var (htmlPath, imgPath) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }

    [Fact]
    public async Task Joined_PivotBy_Method_Dark()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "IterationBenchmark-report-github.md").First();

        var options = new JoinReportHtmlOptions
        {
            Title = "Performance Comparison between for, foreach, and ForEeach() method",
            MainColumn = "Runtime",
            GroupByColumns = ["Categories", "Length"],
            PivotColumn = "Method",
            StatisticColumns = ["Mean"],
            ColumnsOrder = ["for", "foreach", "ForEach()"],
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, imgPath) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }

    [Fact]
    public async Task Joined_PivotBy_Method_Light()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "IterationBenchmark-report-github.md").First();

        var options = new JoinReportHtmlOptions
        {
            Title = "Performance Comparison between for, foreach, and ForEeach() method",
            MainColumn = "Runtime",
            GroupByColumns = ["Categories", "Length"],
            PivotColumn = "Method",
            StatisticColumns = ["Mean"],
            ColumnsOrder = ["for", "foreach", "ForEach()"],
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Light
        };

        var (htmlPath, imgPath) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }
}
