```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3958/23H2/2023Update/SunValley3)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.304
  [Host]   : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method      | Categories          | Mean       | Error       | StdDev   | Allocated |
|------------ |-------------------- |-----------:|------------:|---------:|----------:|
| Serialize   | NewtonsoftJson      | 1,956.6 ns |   588.60 ns | 32.26 ns |    3048 B |
| Deserialize | NewtonsoftJson      | 3,557.3 ns | 1,517.05 ns | 83.15 ns |    5504 B |
| Serialize   | SystemTextJson      | 1,185.5 ns |   101.66 ns |  5.57 ns |    1040 B |
| Deserialize | SystemTextJson      | 2,002.2 ns |   193.02 ns | 10.58 ns |    1784 B |
| Serialize   | SystemTextSourceGen |   721.9 ns |    96.62 ns |  5.30 ns |     728 B |
| Deserialize | SystemTextSourceGen | 2,093.9 ns |   230.57 ns | 12.64 ns |    1688 B |
