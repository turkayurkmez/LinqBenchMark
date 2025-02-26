// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using LinqBenchMark;

//DataSeeder.SeedData();
BenchmarkRunner.Run<EFCoreBenchmarks>();
