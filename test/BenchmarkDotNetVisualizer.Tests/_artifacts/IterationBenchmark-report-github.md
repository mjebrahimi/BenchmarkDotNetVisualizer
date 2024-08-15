```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3958/23H2/2023Update/SunValley3)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.304
  [Host]                 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  ShortRun-.NET 5.0      : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2
  ShortRun-.NET 6.0      : .NET 6.0.32 (6.0.3224.31407), X64 RyuJIT AVX2
  ShortRun-.NET 7.0      : .NET 7.0.20 (7.0.2024.26716), X64 RyuJIT AVX2
  ShortRun-.NET 8.0      : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  ShortRun-.NET Core 3.0 : .NET Core 3.0.3 (CoreCLR 4.700.20.6603, CoreFX 4.700.20.6701), X64 RyuJIT AVX2
  ShortRun-.NET Core 3.1 : .NET Core 3.1.32 (CoreCLR 4.700.22.55902, CoreFX 4.700.22.56512), X64 RyuJIT AVX2

IterationCount=3  LaunchCount=1  WarmupCount=3  

```
| Method    | Runtime       | Categories | Length | Mean       | Error       | StdDev    | Allocated |
|---------- |-------------- |----------- |------- |-----------:|------------:|----------:|----------:|
| for       | .NET 5.0      | Array      | 1000   |   246.7 ns |    30.46 ns |   1.67 ns |         - |
| foreach   | .NET 5.0      | Array      | 1000   |   240.5 ns |    52.98 ns |   2.90 ns |         - |
| ForEach() | .NET 5.0      | Array      | 1000   | 1,444.4 ns |   324.50 ns |  17.79 ns |         - |
| for       | .NET 6.0      | Array      | 1000   |   245.4 ns |    84.32 ns |   4.62 ns |         - |
| foreach   | .NET 6.0      | Array      | 1000   |   243.3 ns |    26.15 ns |   1.43 ns |         - |
| ForEach() | .NET 6.0      | Array      | 1000   | 1,434.5 ns |   130.43 ns |   7.15 ns |         - |
| for       | .NET 7.0      | Array      | 1000   |   243.9 ns |    47.43 ns |   2.60 ns |         - |
| foreach   | .NET 7.0      | Array      | 1000   |   239.2 ns |    34.23 ns |   1.88 ns |         - |
| ForEach() | .NET 7.0      | Array      | 1000   | 1,432.3 ns |   109.75 ns |   6.02 ns |         - |
| for       | .NET 8.0      | Array      | 1000   |   242.3 ns |     5.06 ns |   0.28 ns |         - |
| foreach   | .NET 8.0      | Array      | 1000   |   237.9 ns |    16.09 ns |   0.88 ns |         - |
| ForEach() | .NET 8.0      | Array      | 1000   |   243.6 ns |     1.51 ns |   0.08 ns |         - |
| for       | .NET Core 3.0 | Array      | 1000   |   293.0 ns |    18.81 ns |   1.03 ns |         - |
| foreach   | .NET Core 3.0 | Array      | 1000   |   476.0 ns |    11.86 ns |   0.65 ns |         - |
| ForEach() | .NET Core 3.0 | Array      | 1000   | 1,433.8 ns |    28.19 ns |   1.55 ns |         - |
| for       | .NET Core 3.1 | Array      | 1000   |   294.6 ns |    88.04 ns |   4.83 ns |         - |
| foreach   | .NET Core 3.1 | Array      | 1000   |   242.9 ns |    65.42 ns |   3.59 ns |         - |
| ForEach() | .NET Core 3.1 | Array      | 1000   | 1,674.4 ns |   176.58 ns |   9.68 ns |         - |
| for       | .NET 5.0      | List       | 1000   |   478.0 ns |     8.57 ns |   0.47 ns |         - |
| foreach   | .NET 5.0      | List       | 1000   | 1,882.4 ns |   579.49 ns |  31.76 ns |         - |
| ForEach() | .NET 5.0      | List       | 1000   | 1,684.6 ns |    74.63 ns |   4.09 ns |         - |
| for       | .NET 6.0      | List       | 1000   |   392.4 ns |   887.03 ns |  48.62 ns |         - |
| foreach   | .NET 6.0      | List       | 1000   |   718.4 ns |    48.25 ns |   2.64 ns |         - |
| ForEach() | .NET 6.0      | List       | 1000   | 1,675.2 ns |    26.74 ns |   1.47 ns |         - |
| for       | .NET 7.0      | List       | 1000   |   477.6 ns |    10.90 ns |   0.60 ns |         - |
| foreach   | .NET 7.0      | List       | 1000   |   489.2 ns |    50.86 ns |   2.79 ns |         - |
| ForEach() | .NET 7.0      | List       | 1000   | 1,572.6 ns |   697.66 ns |  38.24 ns |         - |
| for       | .NET 8.0      | List       | 1000   |   394.7 ns | 1,003.36 ns |  55.00 ns |         - |
| foreach   | .NET 8.0      | List       | 1000   |   480.1 ns |    30.16 ns |   1.65 ns |         - |
| ForEach() | .NET 8.0      | List       | 1000   |   619.3 ns |    77.13 ns |   4.23 ns |         - |
| for       | .NET Core 3.0 | List       | 1000   |   480.3 ns |    12.57 ns |   0.69 ns |         - |
| foreach   | .NET Core 3.0 | List       | 1000   | 1,759.1 ns | 1,965.96 ns | 107.76 ns |         - |
| ForEach() | .NET Core 3.0 | List       | 1000   | 1,562.5 ns | 1,827.30 ns | 100.16 ns |         - |
| for       | .NET Core 3.1 | List       | 1000   |   479.9 ns |    19.59 ns |   1.07 ns |         - |
| foreach   | .NET Core 3.1 | List       | 1000   | 1,693.5 ns |    12.95 ns |   0.71 ns |         - |
| ForEach() | .NET Core 3.1 | List       | 1000   | 1,737.7 ns | 1,531.54 ns |  83.95 ns |         - |
