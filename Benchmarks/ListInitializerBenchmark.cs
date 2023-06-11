using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Mocks;

namespace Benchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[MeanColumn, MemoryDiagnoser]
public class ListInitializerBenchmark
{
    [Benchmark]
    public int AddRange()
    {
        var list = new List<ClassMock>();
        list.AddRange(_values);
        return list.Count;
    }
    
    [Benchmark]
    public int AsEnumerable()
    {
        var list = new List<ClassMock>(_values);
        
        return list.Count;
    }

    [Benchmark(Baseline = true)]
    public int Initializer()
    {
        var values = _values;
        var list = new List<ClassMock>
        {
            values[0],
            values[1],
            values[2],
            values[3],
            values[4],
        };

        return list.Count;
    }

    [Benchmark]
    public int ToList()
    {
        var list = _values.ToList();
        return list.Count;
    }
    
    [Benchmark]
    public int WithoutInitializer()
    {
        var values = _values;
        var list = new List<ClassMock>();

        list.Add(values[0]);
        list.Add(values[1]);
        list.Add(values[2]);
        list.Add(values[3]);
        list.Add(values[4]);

        return list.Count;
    }

    private ClassMock[] _values = null!;

    [GlobalSetup]
    public void Init()
    {
        _values = new ClassMock[5];
        for (var i = 0; i < _values.Length; i++)
        {
            _values[i] = new ClassMock(
                new[] {new object(), new object()},
                true,
                i,
                new object());
        }
    }
}