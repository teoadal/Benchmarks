using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Benchmarks.Mapping;

internal sealed class Mapper : IMapper
{
    private readonly InputMappers[] _mappers;

    public Mapper(IEnumerable<ITypeMapper> mappers)
    {
        var result = Array.Empty<InputMappers>();
        var inputMappers = mappers.Select(mapper =>
        {
            foreach (var mapperInterface in mapper.GetType().GetInterfaces())
            {
                if (!mapperInterface.IsGenericType ||
                    mapperInterface.GetGenericTypeDefinition() != typeof(IMapper<,>)) continue;

                var genericArguments = mapperInterface.GetGenericArguments();
                return (
                    input: MapperType.GetId(genericArguments[0]),
                    output: MapperType.GetId(genericArguments[1]), mapper);
            }

            throw new Exception();
        }).GroupBy(tuple => tuple.Item1, tuple => (tuple.output, tuple.mapper));

        foreach (var inputMapper in inputMappers)
        {
            var outputMappers = Array.Empty<ITypeMapper?>();
            foreach (var (index, mapper) in inputMapper)
            {
                CollectionUtils.EnsureCapacity(ref outputMappers, index + 1);

                ref var outputMapper = ref outputMappers[index];
                if (outputMapper != null) MapperErrors.MapperAlreadyExists(mapper);
                outputMappers[index] = mapper;
            }

            var inputId = inputMapper.Key;
            CollectionUtils.EnsureCapacity(ref result, inputId + 1);
            result[inputId] = new InputMappers(outputMappers);
        }

        _mappers = result.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMapper<TIn, TOut>? GetMapper<TIn, TOut>()
    {
        var id = MapperType<TIn>.Id;

        var mappers = _mappers;
        if ((uint) id >= (uint) mappers.Length) return null;

        var inputMapper = mappers[id];
        return inputMapper.IsEmpty
            ? null
            : (IMapper<TIn, TOut>?) inputMapper.GetMapper(MapperType<TOut>.Id);
    }

    public TOut Map<TIn, TOut>(TIn input)
    {
        var mapper = GetMapper<TIn, TOut>();
        if (mapper == null) MapperErrors.MapperNotFound(typeof(TIn), typeof(TOut));
        return mapper!.Map(this, input);
    }

    private readonly struct InputMappers
    {
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _outputMappers == null;
        }

        private readonly ITypeMapper?[] _outputMappers;

        public InputMappers(ITypeMapper?[] outputMappers)
        {
            _outputMappers = outputMappers;
        }

        public ITypeMapper? GetMapper(int outputId)
        {
            var mappers = _outputMappers;
            return (uint) outputId < (uint) mappers.Length
                ? mappers[outputId]
                : null;
        }
    }
}