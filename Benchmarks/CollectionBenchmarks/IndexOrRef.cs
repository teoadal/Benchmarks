using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.CollectionBenchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[MeanColumn, MemoryDiagnoser]
public class IndexOrRef
{
    private int[] _ids = Array.Empty<int>();
    private ClosedStorage _storage = null!;

    [Benchmark]
    public int ByRef()
    {
        var result = 0;
        var storage = _storage;

        foreach (var id in _ids)
        {
            var reference = storage.TryGetItemRef(id);
            if (!reference) continue;

            ref var value = ref reference.Value;
            result += value;
            value = reference.Name.Length;
        }

        return result;
    }

    [Benchmark]
    public int ByRefStructConstraint()
    {
        var result = 0;
        var storage = _storage;

        foreach (var id in _ids)
        {
            var reference = storage.TryGetItemRefStructConstraint(id);
            if (!reference) continue;

            ref var value = ref reference.Value;
            result += value;
            value = reference.Name.Length;
        }

        return result;
    }
    
    [Benchmark]
    public int ByTuple()
    {
        var result = 0;
        var storage = _storage;

        foreach (var id in _ids)
        {
            var (index, name, speed) = storage.GetValue(id);
            if (index == -1) continue;

            result += speed;
            storage[index] = name.Length;
        }

        return result;
    }

    [Benchmark(Baseline = true)]
    public int WithoutRef()
    {
        var result = 0;
        var storage = _storage;

        foreach (var id in _ids)
        {
            if (!storage.TryGetValue(id, out var index, out var name, out var speed)) continue;

            result += speed;
            storage[index] = name.Length;
        }

        return result;
    }

    [GlobalSetup]
    public void Init()
    {
        var random = new Random(123);
        var ids = Enumerable.Range(0, 100).Select(_ => random.Next(-200, 200)).ToArray();

        _ids = CollectionUtils.Shuffle(ids, random);
        _storage = new ClosedStorage(ids);
    }
}