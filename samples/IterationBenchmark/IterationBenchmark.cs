using BenchmarkDotNet.Attributes;

// Using Exporter Attributes
//[BenchmarkDotNetVisualizer.RichImageExporter(title: "Performance Comparison between for, foreach, and ForEeach() method", groupByColumns: ["Runtime"], spectrumColumns: ["Mean", "Allocated"], theme: Theme.Dark)]
//[BenchmarkDotNetVisualizer.RichHtmlExporter(title: "Performance Comparison between for, foreach, and ForEeach() method", groupByColumns: ["Runtime"], spectrumColumns: ["Mean", "Allocated"], theme: Theme.Dark)]
//[BenchmarkDotNetVisualizer.RichMarkdownExporter(title: "Performance Comparison between for, foreach, and ForEeach() method", groupByColumns: ["Runtime"], sortByColumns: ["Mean", "Allocated"])]

#if RELEASE
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp30)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp31)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net50)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net60)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net70)]
[ShortRunJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
#endif
[CategoriesColumn]
[HideColumns("Job")]
[MemoryDiagnoser(displayGenColumns: false)]
public class IterationBenchmark
{
    [Params(1000)]
    public int Length { get; set; }

    private int[] _array = null!;
    private List<int> _list = null!;

    [GlobalSetup]
    public void Setup()
    {
        _array = Enumerable.Range(0, Length).ToArray();
        _list = Enumerable.Range(0, Length).ToList();
    }

    [Benchmark(Description = "for"), BenchmarkCategory("Array")]
    public void ArrayFor()
    {
        var length = _array.Length;
        for (int i = 0; i < length; i++)
        {
            _ = _array[i];
        }
    }

    [Benchmark(Description = "foreach"), BenchmarkCategory("Array")]
    public void ArrayForEach()
    {
        foreach (var _ in _array)
        {
        }
    }

    [Benchmark(Description = "ForEach()"), BenchmarkCategory("Array")]
    public void ArrayForEachMethod()
    {
        Array.ForEach(_array, _ => { });
    }

    [Benchmark(Description = "for"), BenchmarkCategory("List")]
    public void ListFor()
    {
        var length = _list.Count;
        for (int i = 0; i < length; i++)
        {
            _ = _list[i];
        }
    }

    [Benchmark(Description = "foreach"), BenchmarkCategory("List")]
    public void ListForEach()
    {
        foreach (var _ in _list)
        {
        }
    }

    [Benchmark(Description = "ForEach()"), BenchmarkCategory("List")]
    public void ListForEachMethod()
    {
        _list.ForEach(_ => { });
    }
}