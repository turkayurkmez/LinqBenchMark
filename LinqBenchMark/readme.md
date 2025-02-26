```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5487/22H2/2022Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2


```
| Method             | Mean     | Error   | StdDev  | Gen0      | Gen1      | Gen2     | Allocated |
|------------------- |---------:|--------:|--------:|----------:|----------:|---------:|----------:|
| Normal             | 158.5 ms | 2.62 ms | 2.33 ms | 5000.0000 | 1333.3333 |        - |  30.65 MB |
| WithOptimizedYield | 103.9 ms | 2.06 ms | 2.20 ms | 7000.0000 |         - |        - |  21.01 MB |
| withBufferAsync    | 153.3 ms | 2.88 ms | 3.20 ms | 5000.0000 | 1500.0000 | 500.0000 |  31.71 MB |
| WithSkipTake       | 125.6 ms | 2.47 ms | 2.31 ms |  666.6667 |         - |        - |   4.36 MB |
