namespace BenchmarkDotNetVisualizer.Tests;

public class JsonSerializersBenchmarkTests : TestBase
{
    [Fact]
    public async Task Simple_Dark()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "JsonSerializersBenchmark-report-github.md").First();

        var options = new ReportHtmlOptions
        {
            Title = "Json Serializers Benchmark",
            GroupByColumns = ["Method"],
            SpectrumColumns = ["Mean", "Allocated"],
            DividerMode = RenderTableDividerMode.EmptyDividerRow,
            HtmlWrapMode = HtmlDocumentWrapMode.Simple,
            Theme = Theme.Dark
        };

        var (htmlPath, _) = await SaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Simple_Light()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "JsonSerializersBenchmark-report-github.md").First();

        var options = new ReportHtmlOptions
        {
            Title = "Json Serializers Benchmark",
            GroupByColumns = ["Method"],
            SpectrumColumns = ["Mean", "Allocated"],
            DividerMode = RenderTableDividerMode.EmptyDividerRow,
            HtmlWrapMode = HtmlDocumentWrapMode.Simple,
            Theme = Theme.Light
        };

        var (htmlPath, _) = await SaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }
}
