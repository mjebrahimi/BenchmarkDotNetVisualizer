[![NuGet](https://buildstats.info/nuget/BenchmarkDotNetVisualizer)](https://www.nuget.org/packages/BenchmarkDotNetVisualizer)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/mjebrahimi/BenchmarkDotNetVisualizer/workflows/.NET/badge.svg)](https://github.com/mjebrahimi/BenchmarkDotNetVisualizer)

# BenchmarkDotNetVisualizer

Visualizes your [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet/) benchmarks to **Colorful** images, **Feature-rich** HTML, and customizable markdown files (and maybe powerful charts in the future!)

**In Simple Words:** You can create something like this 👇

![for, foreach, ForEach() Benchmark](https://raw.githubusercontent.com/mjebrahimi/BenchmarkDotNetVisualizer/master/Demo.png)

## Table of Content

- [A Real World Demo (.NET Collections Benchmark 🚀)](#a-real-world-demo-net-collections-benchmark-)
- [Getting Started](#getting-started)
  - [1. Install Package](#1-install-package)
  - [2. Simple Using](#2-simple-using)
  - [3. Using Exporters](#3-using-exporters)
- [Using BenchmarkAutoRunner to Run your benchmarks](#using-benchmarkautorunner-to-run-your-benchmarks)
- [Using JoinReports method to Join and Pivot your reports](#using-joinreports-method-to-join-and-pivot-your-reports)
  - [Pivot by .NET Runtime version column](#pivot-by-net-runtime-version-column)
  - [Pivot by Method column](#pivot-by-method-column)
- [Todo and Contribution](#todo)

## A Real World Demo (.NET Collections Benchmark 🚀)

A Comprehensive Performance Comparison Benchmark Between Different .NET Collections.

https://github.com/mjebrahimi/DotNet-Collections-Benchmark/

## Getting Started

### 1. Install Package

```ini
PM> Install-Package BenchmarkDotNetVisualizer
```

### 2. Simple Using

**Methods:**

- **SaveAsImageAsync()**
- **SaveAsHtmlAsync()**
- **SaveAsHtmlAndImageAsync()**

**Example:**

```csharp
var summary = BenchmarkRunner.Run<JsonSerializersBenchmark>(); 

//[ProjectDirectory]\Reports\JsonSerializers\Benchmark.html
var htmlFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\JsonSerializers\Benchmark.html");

//[ProjectDirectory]\Reports\JsonSerializers\Benchmark.png
var imageFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\JsonSerializers\Benchmark.png");
await summary.SaveAsHtmlAndImageAsync(
    htmlPath: htmlFileName, 
    imagePath: imageFileName,
    options: new ReportHtmlOptions
    {
        Title = "Json Serializers Benchmark",
        GroupByColumns = ["Method"],                          // Groups by 'Method' column and highlights groups
        SpectrumColumns = ["Mean", "Allocated"],              // Colorizes 'Mean' and 'Allocated' columns as Spectrum
        DividerMode = RenderTableDividerMode.EmptyDividerRow, // Separates tables by Empty Divider Row
        HtmlWrapMode = HtmlDocumentWrapMode.Simple            // Uses simple HTML table
    });
```

**Output HTML:**
Visit [this HTML page](https://mjebrahimi.github.io/BenchmarkDotNetVisualizer/json-serializers-benchmark.html) at `samples/Reports/JsonSerializers/Benchmark.html`

**Output Image:**
![Json Serializers Benchmark](https://raw.githubusercontent.com/mjebrahimi/BenchmarkDotNetVisualizer/master/samples/Reports/JsonSerializers/Benchmark.png)

### 3. Using Exporters

**Exporters:**

- **[RichImageExporter]**
- **[RichHtmlExporter]**

**Example:**

```csharp
BenchmarkRunner.Run<JsonSerializersBenchmark>();

//Exports colorful image
[RichImageExporter(
    title: "Json Serializers Benchmark", 
    groupByColumns: ["Method"],             // Groups by 'Method' column and highlights groups
    spectrumColumns: ["Mean", "Allocated"], // Colorizes 'Mean' and 'Allocated' columns as Spectrum and Sorts the result by them 
    //format: ImageFormat.Webp or Jpeg      // You can set image format (Default is ImageFormat.Png)
)]  

//Exports feature-rich HTML
[RichHtmlExporter(
    title: "Json Serializers Benchmark", 
    groupByColumns: ["Method"],             // Groups by 'Method' column and highlights groups
    spectrumColumns: ["Mean", "Allocated"]  // Colorizes 'Mean' and 'Allocated' columns as Spectrum and Sorts the result by them 
    //sortByColumns: ["Mean", "Allocated"]  // You can also sort by other columns as you wish
)]

[MemoryDiagnoser(displayGenColumns: false)] // Displays Allocated column (without GC per Generation columns (Gen 0, Gen 1, Gen 2) due to false option)
public class JsonSerializersBenchmark { ... }
```

**Output:**

To see the results, navigate to the following path:

`[ProjectDirectory]\bin\[Debug|Release]\[.NET-version]\BenchmarkDotNet.Artifacts\results\Benchmark-report-rich.html|png`

**For Example:**

- `MyBenchmark\bin\Release\net8.0\BenchmarkDotNet.Artifacts\results\Benchmark-report-rich.png`
- `MyBenchmark\bin\Release\net8.0\BenchmarkDotNet.Artifacts\results\Benchmark-report-rich.html`

### Using BenchmarkAutoRunner to Run your benchmarks

It's **Recommend** to use **BenchmarkAutoRunner.Run()** instead of **BenchmarkRunner.Run()** to run your benchmarks.

`BenchmarkAutoRunner` is **similar** to `BenchmarkRunner`, but uses `Job.Dry` with `InProcessEmitToolchain` is case of **DEBUG** Mode (due to **ease of debugging**), and your **specified job** in case of **RELEASE** Mode.

It also **Warns** you if you are running project **incorrectly**. (for example running with **Attached Debugger** while **RELEASE Mode is enabled**)

```csharp
var summary = BenchmarkAutoRunner.Run<IteratorsBenchmark>();
```

### Using JoinReports method to Join and Pivot your reports

**Example:**

**Performance benchmark between for, foreach, and ForEach() in different versions of .NET**

#### Pivot by .NET Runtime version column

```csharp
//Recommend to use BenchmarkAutoRunner instead of BenchmarkRunner
var summary = BenchmarkAutoRunner.Run<IteratorsBenchmark>();

//[ProjectDirectory]\Reports\Iterators\JoinedBenchmark-PivotBy-Runtime.html
var htmlFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\JoinedBenchmark-PivotBy-Runtime.html");

//[ProjectDirectory]\Reports\Iterators\JoinedBenchmark-PivotBy-Runtime.png
var imageFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\JoinedBenchmark-PivotBy-Runtime.png");

await summary.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: htmlFileName,
    imagePath: imageFileName,
    options: new JoinReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEach() method",
        MainColumn = "Method",
        GroupByColumns = ["Categories", "Length"],           // Groups by column 'Categories' and 'Length'
        PivotProperty = "Runtime",
        StatisticColumns = ["Mean"],
        ColumnsOrder = [".NET Core 3.0", ".NET Core 3.1", ".NET 5.0", ".NET 6.0", ".NET 7.0", ".NET 8.0"], // Order of columns 
        DividerMode = RenderTableDividerMode.SeparateTables, //Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  //Uses feature-rich https://datatables.net plugin
    });
```

**Output HTML:**
Visit [this HTML page](https://mjebrahimi.github.io/BenchmarkDotNetVisualizer/iterators-benchmark2.html) at `samples/Reports/Iterators/JoinedBenchmark-PivotBy-Runtime.html`

**Output Image:**
![Iterators Benchmark](https://raw.githubusercontent.com/mjebrahimi/BenchmarkDotNetVisualizer/master/samples/Reports/Iterators/JoinedBenchmark-PivotBy-Runtime.png)


#### Pivot by Method column

```csharp
//Recommend to use BenchmarkAutoRunner instead of BenchmarkRunner
var summary = BenchmarkAutoRunner.Run<IteratorsBenchmark>();

//[ProjectDirectory]\Reports\Iterators\JoinedBenchmark-PivotBy-Method.html
var htmlFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\JoinedBenchmark-PivotBy-Method.html");

//[ProjectDirectory]\Reports\Iterators\JoinedBenchmark-PivotBy-Method.png
var imageFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\JoinedBenchmark-PivotBy-Method.png");

await summary2.JoinReportsAndSaveAsHtmlAndImageAsync(
    htmlPath: htmlFileName,
    imagePath: imageFileName,
    options: new JoinReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEach() method",
        MainColumn = "Runtime",
        GroupByColumns = ["Categories", "Length"],           // Groups by column 'Categories' and 'Length'
        PivotProperty = "Method",
        StatisticColumns = ["Mean"],
        ColumnsOrder = ["for", "foreach", "ForEach()"],      // Order of columns 
        DividerMode = RenderTableDividerMode.SeparateTables, // Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  // Uses feature-rich https://datatables.net plugin
    });
```

**Output HTML:**
Visit [this HTML page](https://mjebrahimi.github.io/BenchmarkDotNetVisualizer/iterators-benchmark3.html) at `samples/Reports/Iterators/JoinedBenchmark-PivotBy-Method.html`

**Output Image:**
![Iterators Benchmark](https://raw.githubusercontent.com/mjebrahimi/BenchmarkDotNetVisualizer/master/samples/Reports/Iterators/JoinedBenchmark-PivotBy-Method.png)

<!-- 
### Performance Benchmark between for, foreach, and ForEach() method

```csharp
//Recommend to use BenchmarkAutoRunner instead of BenchmarkRunner
var summary = BenchmarkAutoRunner.Run<IteratorsBenchmark>();

//[ProjectDirectory]\Reports\Iterators\Benchmark.html
var htmlFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\Benchmark.html");

//[ProjectDirectory]\Reports\Iterators\Benchmark.png
var imageFileName = DirectoryHelper.GetPathRelativeToProjectDirectory(@"Reports\Iterators\Benchmark.png");

await summary.SaveAsHtmlAndImageAsync(
    htmlPath: htmlFileName,
    imagePath: imageFileName,
    options: new ReportHtmlOptions
    {
        Title = "Performance Comparison between for, foreach, and ForEach() method",
        GroupByColumns = ["Runtime"],                        // Groups by column 'Runtime'
        SpectrumColumns = ["Mean", "Allocated"],             // Colorizes 'Mean' and 'Allocated' columns as Spectrum
        DividerMode = RenderTableDividerMode.SeparateTables, // Separates tables by Grouping by 'GroupByColumns'
        HtmlWrapMode = HtmlDocumentWrapMode.RichDataTables,  // Uses feature-rich https://datatables.net plugin
    });
```

**Output HTML:**
Visit [this HTML page](https://mjebrahimi.github.io/BenchmarkDotNetVisualizer/iterators-benchmark1.html) at `samples/Reports/Iterators/Benchmark.html`

**Output Image:**
![Iterators Benchmark](https://raw.githubusercontent.com/mjebrahimi/BenchmarkDotNetVisualizer/master/samples/Reports/Iterators/Benchmark.png)
-->

## Todo

- [ ] Dark Theme (Need some help for this, wanna help? Please design a beautiful style for dark theme and send a PR)
- [ ] Chart Visualization

## Contributing

Create an [issue](https://github.com/mjebrahimi/BenchmarkDotNetVisualizer/issues/new) if you find a BUG or have a Suggestion or Question.

If you want to develop this project :

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## Give it a Star! ⭐️

If you find this repository useful and like it, why not give it a star? if not, never mind! :)

## License

Copyright © 2024 [Mohammad Javad Ebrahimi](https://github.com/mjebrahimi) under the [MIT License](https://github.com/mjebrahimi/BenchmarkDotNetVisualizer/LICENSE).
