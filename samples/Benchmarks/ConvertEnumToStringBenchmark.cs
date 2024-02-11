using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace BenchmarkDotNetVisualizer.Demo.Benchmarks;


[RichImageExporter(title: "Converting Enum To A String Benchmarks", groupByColumns: ["Method"], spectrumColumns: ["Mean", "Allocated"])]
[RichHtmlExporter(title: "Converting Enum To A String Benchmarks", groupByColumns: ["Method"], spectrumColumns: ["Mean", "Allocated"])]
[RichMarkdownExporter(title: "Converting Enum To A String Benchmarks", groupByColumns: ["Method"], sortByColumns: ["Mean", "Allocated"])]

#if RELEASE
[ShortRunJob]
#endif

[MemoryDiagnoser(displayGenColumns: false)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ConvertEnumToStringBenchmark
{
    [Benchmark(Description = "ToString()")]
    public string ConvertUsingToStringMethod()
    {
        return MyEnum.ALongAndVerboseEnumName.ToString();
    }

    [Benchmark(Description = "NameOf()")]
    public string ConvertUsingNameOfMethod() 
    {
        return nameof(MyEnum.ALongAndVerboseEnumName);
    }

    [Benchmark(Description = "Enum.GetName()")]
    public string ConvertUsingSecondOverloadOfGetNameMethod()
    {
        return Enum.GetName(typeof(MyEnum), MyEnum.ALongAndVerboseEnumName)!;
    }
}

public enum MyEnum
{
    ALongAndVerboseEnumName
}