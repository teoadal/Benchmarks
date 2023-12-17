using System.Collections.Concurrent;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.StringBenchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MeanColumn, MemoryDiagnoser]
public class InternBenchmarks
{
    [Benchmark(Baseline = true)]
    public int Intern() => _values.Sum(value => string.Intern(value.ToString()).Length);

    [Benchmark]
    public int Concurrent() => _values.Sum(value => _cache.GetOrAdd(value.ToString(), v => v).Length);

    private ConcurrentDictionary<string, string> _cache = new();
    private int[] _values = null!;

    [GlobalSetup]
    public void Config()
    {
        _values = Enumerable
            .Range(0, 128)
            .Select(value => value * 1024)
            .ToArray();
    }
}