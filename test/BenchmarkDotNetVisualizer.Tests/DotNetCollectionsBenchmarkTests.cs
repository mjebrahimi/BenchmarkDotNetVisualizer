using BenchmarkDotNetVisualizer.Utilities;

namespace BenchmarkDotNetVisualizer.Tests;

public class DotNetCollectionsBenchmarkTests : TestBase
{
    [Fact]
    public async Task Benchmark_Initialize_Mean()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.InitializeBenchmark*-report-github.md").ToArray();

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Initializing in terms of Execution Time (Mean)",
            MainColumn = "Method",
            GroupByColumns = ["Length", "Sorted"],
            PivotColumn = "DataType",
            StatisticColumns = ["Mean"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Benchmark_Initialize_Allocated()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.InitializeBenchmark*-report-github.md").ToArray();

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Initializing in terms of Allocation Size",
            MainColumn = "Method",
            GroupByColumns = ["Length", "Sorted"],
            PivotColumn = "DataType",
            StatisticColumns = ["Allocated"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Benchmark_SearchContains_Mean()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.ContainsBenchmark*-report-github.md").ToArray();

        ChangeColorAndNameOfBigOColumn(benchmarkInfo);

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Searching (Contains method) in terms of Execution Time (Mean)",
            MainColumn = "Method",
            GroupByColumns = ["Existed"],
            PivotColumn = "DataType",
            StatisticColumns = ["Mean"],
            OtherColumnsToSelect = ["Big O", "Length"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Benchmark_SearchContains_Allocated()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.ContainsBenchmark*-report-github.md").ToArray();

        ChangeColorAndNameOfBigOColumn(benchmarkInfo);

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Searching (Contains method) in terms of Allocation Size",
            MainColumn = "Method",
            GroupByColumns = ["Existed"],
            PivotColumn = "DataType",
            StatisticColumns = ["Allocated"],
            OtherColumnsToSelect = ["Big O", "Length"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
            Theme = Theme.Dark
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Benchmark_SearchTryGetValue_Mean()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.TryGetValueBenchmark*-report-github.md").ToArray();

        ChangeColorAndNameOfBigOColumn(benchmarkInfo);

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Searching (TryGetValue method) in terms of Execution Time (Mean)",
            MainColumn = "Method",
            GroupByColumns = ["Existed"],
            PivotColumn = "DataType",
            StatisticColumns = ["Mean"],
            OtherColumnsToSelect = ["Big O", "Length"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    [Fact]
    public async Task Benchmark_SearchTryGetValue_Allocated()
    {
        var benchmarkInfo = BenchmarkInfo.CreateFromDirectory(_benchmarkArtifactsPath, searchPattern: "Benchmarks.TryGetValueBenchmark*-report-github.md").ToArray();

        ChangeColorAndNameOfBigOColumn(benchmarkInfo);

        var options = new JoinReportHtmlOptions()
        {
            Title = "Benchmark of Collection Searching (TryGetValue method) in terms of Allocation Size",
            MainColumn = "Method",
            GroupByColumns = ["Existed"],
            PivotColumn = "DataType",
            StatisticColumns = ["Allocated"],
            OtherColumnsToSelect = ["Big O", "Length"],
            ColumnsOrder = ["Int32", "String", "StructInts", "ClassInts", "RecordStructInts", "RecordClassInts", "StructStrings", "ClassStrings", "RecordStructStrings", "RecordClassStrings"],
            SpectrumStatisticColumn = true,
            HighlightGroups = true,
            DividerMode = RenderTableDividerMode.SeparateTables,
            HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,
        };

        var (htmlPath, _) = await JoinReportsAndSaveAsHtmlAndImageAsync(benchmarkInfo, options);

        await VerifyHtml(htmlPath);
    }

    #region Utils
    private static void ChangeColorAndNameOfBigOColumn(BenchmarkInfo[] benchmarkInfo)
    {
        const string srcPropertyName = "Categories";
        const string destPropertyName = "Big O";

        foreach (var item in benchmarkInfo)
        {
            item.Table = item.Table.Select(expando =>
            {
                if (expando is not null)
                {
                    expando.ChangePropertyName(srcPropertyName, destPropertyName);
                    expando.TransferColumnOrder(srcPropertyName, destPropertyName);
                    var value = expando.GetProperty(destPropertyName)!.ToString()!.RemoveMarkdownBold();
                    var color = GetColor(value);
                    expando.SetMetaProperty($"{destPropertyName}.background-color", color);
                }
                return expando;
            });
        }

        static string GetColor(string value)
        {
            return value switch
            {
                "O(1)" => "#99FF99",
                "O(log(N))" => "#FFFF99",
                "O(N)" => "#FF9999",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"The value '{value}' is not defined.")
            };
        }
    }
    #endregion
}
