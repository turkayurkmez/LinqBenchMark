```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5487/22H2/2022Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2


```
| Method                         | Mean       | Error    | StdDev   | Median     | Gen0       | Gen1      | Gen2     | Allocated |
|------------------------------- |-----------:|---------:|---------:|-----------:|-----------:|----------:|---------:|----------:|
| Normal                         |   193.1 ms |  5.88 ms | 16.59 ms |   188.7 ms |  5000.0000 | 1500.0000 | 500.0000 |  31.15 MB |
| Optimized                      |   150.7 ms |  2.99 ms |  6.31 ms |   148.8 ms | 10000.0000 |         - |        - |  30.15 MB |
| WithBufferAsync                |   165.7 ms |  4.42 ms | 12.54 ms |   161.7 ms |  8333.3333 | 2333.3333 |        - |  30.62 MB |
| WithBulkLoadAndLocalPagination |   206.4 ms |  4.48 ms | 12.94 ms |   202.2 ms |  5500.0000 | 2000.0000 | 500.0000 |  31.63 MB |
| WithCompiledQuery              |   134.5 ms |  2.59 ms |  2.30 ms |   134.3 ms | 10000.0000 |         - |        - |  30.15 MB |
| WithKeySetPagination           | 3,241.3 ms | 40.84 ms | 38.20 ms | 3,242.8 ms | 10000.0000 |         - |        - |  32.48 MB |
