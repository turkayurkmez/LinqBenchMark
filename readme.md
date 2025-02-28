```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5487/22H2/2022Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2


```
| Method             | Mean     | Error   | StdDev  | Gen0      | Gen1      | Gen2     | Allocated |
|------------------- |---------:|--------:|--------:|----------:|----------:|---------:|----------:|
| Normal             | 149.4 ms | 2.98 ms | 4.65 ms | 5000.0000 | 1500.0000 | 500.0000 |  30.65 MB |
| WithOptimizedYield | 101.3 ms | 1.88 ms | 1.76 ms | 7000.0000 |         - |        - |  21.01 MB |
| withBufferAsync    | 146.0 ms | 2.86 ms | 3.92 ms | 5333.3333 | 1666.6667 | 666.6667 |  31.71 MB |
| WithSkipTake       | 115.0 ms | 1.93 ms | 1.80 ms |  750.0000 |  250.0000 |        - |   4.36 MB |
