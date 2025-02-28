```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5487/22H2/2022Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.103
  [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2


```
| Method               | Mean    | Error    | StdDev   | Gen0        | Gen1       | Gen2      | Allocated    |
|--------------------- |--------:|---------:|---------:|------------:|-----------:|----------:|-------------:|
| Normal               | 1.896 s | 0.0324 s | 0.0287 s |  53000.0000 | 16000.0000 | 3000.0000 |  326460.4 KB |
| Optimized            | 1.365 s | 0.0184 s | 0.0172 s | 101000.0000 |          - |         - | 310049.61 KB |
| WithBufferAsync      | 1.387 s | 0.0171 s | 0.0143 s | 103000.0000 |          - |         - | 315304.02 KB |
| WithKeySetPagination | 1.213 s | 0.0095 s | 0.0089 s |           - |          - |         - |    734.65 KB |
