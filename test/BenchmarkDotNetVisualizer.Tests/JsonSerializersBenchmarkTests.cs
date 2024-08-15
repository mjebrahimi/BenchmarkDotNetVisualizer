namespace BenchmarkDotNetVisualizer.Tests;

public class JsonSerializersBenchmarkTests : TestBase
{
    [Fact]
    public async Task Simple_Dark()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "JsonSerializersBenchmark-report-github.md").First();

        var options = new ReportHtmlOptions
        {
            Title = "Json Serializers Benchmark",
            GroupByColumns = ["Method"],
            SpectrumColumns = ["Mean", "Allocated"],
            DividerMode = RenderTableDividerMode.EmptyDividerRow,
            HtmlWrapMode = HtmlDocumentWrapMode.Simple,
            Theme = Theme.Dark
        };

        var (htmlPath, imgPath) = await SaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }

    [Fact]
    public async Task Simple_Light()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_artifactsPath, searchPattern: "JsonSerializersBenchmark-report-github.md").First();

        var options = new ReportHtmlOptions
        {
            Title = "Json Serializers Benchmark",
            GroupByColumns = ["Method"],
            SpectrumColumns = ["Mean", "Allocated"],
            DividerMode = RenderTableDividerMode.EmptyDividerRow,
            HtmlWrapMode = HtmlDocumentWrapMode.Simple,
            Theme = Theme.Light
        };

        var (htmlPath, imgPath) = await SaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtmlAndImage(htmlPath, imgPath);
    }
}
