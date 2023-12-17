using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.ValuesBenchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MeanColumn, MemoryDiagnoser]
public class EmptyGuidBenchmarks
{
    [Benchmark(Baseline = true)]
    public int Generic() => _values.Sum(value => value.TryGetGeneric(out var exists)
        ? exists.ToString().Length
        : -1);

    [Benchmark]
    public int Concrete() => _values.Sum(value => value.TryGet(out var exists)
        ? exists.ToString().Length
        : -1);

    private Guid[] _values = null!;

    [GlobalSetup]
    public void Config()
    {
        _values = Enumerable
            .Range(0, 128)
            .Select(value => value % 2 == 0 ? Guid.NewGuid() : Guid.Empty)
            .ToArray();
    }
}

public static class EmptyGuid
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet(in this Guid guid, out Guid value)
    {
        value = guid;
        return guid != Guid.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetGeneric<T>(this T instance, out T value) where T : struct
    {
        value = instance;
        return !instance.Equals(default);
    }
}