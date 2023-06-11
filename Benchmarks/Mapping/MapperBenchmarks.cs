using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.Mocks;
using Mapster;

namespace Benchmarks.Mapping;

[SimpleJob(RuntimeMoniker.Net70)]
[MeanColumn, MemoryDiagnoser]
public class MapperBenchmarks
{
    [Benchmark(Baseline = true)]
    public int Mapster()
    {
        var result = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var input in _values)
        {
            var to = input.Adapt<OutValue>();
            result += to.Value;
        }

        return result;
    }

    [Benchmark]
    public int MapsterWithFunction()
    {
        var elementMapper = TypeAdapter<FromValue, OutValue>.Map;

        var result = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var input in _values)
        {
            var to = elementMapper(input);
            result += to.Value;
        }

        return result;
    }

    [Benchmark]
    public int MyMapper()
    {
        var result = 0;
        var elementMapper = _myMapper.GetMapper<FromValue, OutValue>()!;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var input in _values)
        {
            var to = elementMapper.Map(_myMapper, input);
            result += to.Value;
        }

        return result;
    }

    [Benchmark]
    public int NormalMapper()
    {
        var result = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var input in _values)
        {
            var to = new OutValue
            {
                Id = input.Id,
                Description = input.Description,
                Name = input.Name,
                Value = input.Value
            };

            result += to.Value;
        }

        return result;
    }

    private IMapper _myMapper = null!;
    private FromValue[] _values = null!;

    [GlobalSetup]
    public void Init()
    {
        _myMapper = new Mapper(new[]
        {
            new ReflectionMapper<FromValue, OutValue>()
        });

        _values = new FromValue[5];
        for (var i = 0; i < _values.Length; i++)
        {
            _values[i] = new FromValue
            {
                Id = Guid.NewGuid(),
                Description = "bbb",
                Name = "abc",
                Value = i
            };
        }
    }
}