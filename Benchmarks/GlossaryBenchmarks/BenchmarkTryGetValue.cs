using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.GlossaryBenchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net48)]
[MeanColumn]
public class BenchmarkTryGetValue
{
    [Benchmark(Baseline = true)]
    public int DictionaryTryGetValue()
    {
        var dictionary = _dictionary;
        var sum = 0;
        foreach (var key in _keys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    [Benchmark]
    public int GlossaryTryGetValue()
    {
        var glossary = _glossary;
        var sum = 0;
        foreach (var key in _keys)
        {
            if (glossary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    [Benchmark]
    public int GlossaryNonSealedTryGetValue()
    {
        var glossary = _glossaryNonSealed;
        var sum = 0;
        foreach (var key in _keys)
        {
            if (glossary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    [Benchmark]
    public int GlossarySealedTryGetValue()
    {
        var glossary = _glossarySealed;
        var sum = 0;
        foreach (var key in _keys)
        {
            if (glossary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    #region Configuration

    private Dictionary<int, int> _dictionary = null!;
    private Glossary<int> _glossary;
    private GlossaryNonSealedClass<int> _glossaryNonSealed = null!;
    private GlossarySealedClass<int> _glossarySealed = null!;
    private int[] _keys = null!;

    [GlobalSetup]
    public void Init()
    {
        const int count = 1021;

        _dictionary = new Dictionary<int, int>(count);
        _glossary = new Glossary<int>(count);
        _glossaryNonSealed = new GlossaryNonSealedClass<int>(count);
        _glossarySealed = new GlossarySealedClass<int>(count);
        _keys = Enumerable.Range(0, count).ToArray();

        for (var i = 0; i < count; i++)
        {
            var number = _keys[i];
            _dictionary.Add(number, number);
            _glossary.Add(number, number);
            _glossaryNonSealed.Add(number, number);
            _glossarySealed.Add(number, number);
        }

        Shuffle(_keys, new Random(1234));
    }

    private static void Shuffle<T>(T[] array, Random random)
    {
        var n = array.Length;
        while (n > 1)
        {
            var k = random.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }

    #endregion
}