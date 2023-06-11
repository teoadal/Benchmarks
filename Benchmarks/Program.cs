using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Benchmarks.Mapping;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());

BenchmarkRunner.Run<MapperBenchmarks>();