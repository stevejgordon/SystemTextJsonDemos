``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview9-014004
  [Host]     : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT


```
|                   Method |         Mean |         Error |        StdDev |       Median |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|------------------------- |-------------:|--------------:|--------------:|-------------:|-------:|-------:|------:|----------:|
|                HighLevel | 865,114.5 ns | 29,099.781 ns | 85,801.360 ns | 912,150.0 ns | 6.8359 | 1.9531 |     - |   31248 B |
|           HighLevelEmpty | 521,730.1 ns | 18,397.991 ns | 54,246.891 ns | 554,526.6 ns | 7.3242 | 3.4180 |     - |   30540 B |
|                 MidLevel |   3,192.9 ns |     69.495 ns |     74.359 ns |   3,172.0 ns | 0.1450 |      - |     - |     624 B |
|            MidLevelEmpty |   1,557.2 ns |     20.582 ns |     19.253 ns |   1,558.3 ns | 0.1068 |      - |     - |     448 B |
|                 LowLevel |   1,667.3 ns |     20.512 ns |     18.183 ns |   1,668.7 ns | 0.1240 |      - |     - |     520 B |
|            LowLevelEmpty |     672.6 ns |      6.150 ns |      5.753 ns |     672.5 ns | 0.0820 |      - |     - |     344 B |
|      LowLevelWithoutPipe |   1,192.4 ns |     23.260 ns |     33.359 ns |   1,193.1 ns | 0.0248 |      - |     - |     104 B |
| LowLevelEmptyWithoutPipe |     284.5 ns |      5.683 ns |     12.111 ns |     284.2 ns |      - |      - |     - |         - |
