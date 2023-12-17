using BenchmarkDotNet.Running;
using Benchmarks.StringBenchmarks;
using Benchmarks.ValuesBenchmarks;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());

BenchmarkRunner.Run<EmptyGuidBenchmarks>();