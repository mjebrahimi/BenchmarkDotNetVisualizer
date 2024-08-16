using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using System.Text.Json;
using System.Text.Json.Serialization;

// Using Exporter Attributes
//[BenchmarkDotNetVisualizer.RichImageExporter(title: "Json Serializers Benchmark", groupByColumns: ["Method"], spectrumColumns: ["Mean", "Allocated"], theme: Theme.Dark)]
//[BenchmarkDotNetVisualizer.RichHtmlExporter(title: "Json Serializers Benchmark", groupByColumns: ["Method"], spectrumColumns: ["Mean", "Allocated"], theme: Theme.Dark)]
//[BenchmarkDotNetVisualizer.RichMarkdownExporter(title: "Json Serializers Benchmark", groupByColumns: ["Method"], sortByColumns: ["Mean", "Allocated"])]

#if RELEASE
[ShortRunJob]
#endif
[CategoriesColumn]
[MemoryDiagnoser(displayGenColumns: false)]
//[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByMethod)]
//[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class JsonSerializersBenchmark
{
    private static readonly Person Person = GeneratePerson();
    private static readonly string Json = GenerateJson();

#pragma warning disable CA1822 // Mark members as static
    #region Serialize
    [Benchmark(Description = "Serialize"), BenchmarkCategory("NewtonsoftJson")]
    public string Serialize1()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(Person);
    }

    [Benchmark(Description = "Serialize"), BenchmarkCategory("SystemTextJson")]
    public string Serialize2()
    {
        return JsonSerializer.Serialize(Person);
    }

    [Benchmark(Description = "Serialize"), BenchmarkCategory("SystemTextSourceGen")]
    public string Serialize3()
    {
        return JsonSerializer.Serialize(Person, AppJsonSerializerContext.Default.Person);
    }
    #endregion

    #region Deserialize
    [Benchmark(Description = "Deserialize"), BenchmarkCategory("NewtonsoftJson")]
    public Person? Deserialize1()
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(Json);
    }

    [Benchmark(Description = "Deserialize"), BenchmarkCategory("SystemTextJson")]
    public Person? Deserialize2()
    {
        return JsonSerializer.Deserialize<Person>(Json);
    }

    [Benchmark(Description = "Deserialize"), BenchmarkCategory("SystemTextSourceGen")]
    public Person? Deserialize3()
    {
        return JsonSerializer.Deserialize(Json, AppJsonSerializerContext.Default.Person);
    }
    #endregion
#pragma warning restore CA1822 // Mark members as static

    #region private methods
    private static Person GeneratePerson()
    {
        return new
        (
            Name: "John",
            Age: 30,
            Gender: "Male",
#pragma warning disable S6562 // Always set the "DateTimeKind" when creating new "DateTime" instances
            DateOfBirth: new DateTime(1994, 1, 1),
#pragma warning restore S6562 // Always set the "DateTimeKind" when creating new "DateTime" instances
            PlaceOfBirth: "City, Country",
            ContactInfo: new
            (
                PhoneNumber: "1234567890",
                Email: "example@example.com"
            ),
            PhysicalDescription: new
            (
                Height: 170,
                Weight: 70,
                HairColor: "Black",
                EyeColor: "Brown"
            ),
            Family: new
            (
                Parents: ["Parent 1", "Parent 2"],
                Siblings: ["Sibling 1", "Sibling 2"]
            )
        );
    }

    private static string GenerateJson()
    {
        var person = GeneratePerson();
        return Newtonsoft.Json.JsonConvert.SerializeObject(person);
    }
    #endregion
}

public record Person(string Name, int Age, string Gender, DateTime DateOfBirth, string PlaceOfBirth, ContactInformation ContactInfo, PhysicalDescription PhysicalDescription, FamilyInfo Family);
public record ContactInformation(string PhoneNumber, string Email);
public record PhysicalDescription(double Height, double Weight, string HairColor, string EyeColor);
public record FamilyInfo(string[] Parents, string[] Siblings);

[JsonSerializable(typeof(Person))]
public partial class AppJsonSerializerContext : JsonSerializerContext;