using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Benchmarks.Mapping;

internal static class MapperType
{
    private static readonly ConcurrentDictionary<Type, int> MapperTypes = new();
    private static int _nextId = -1;

    public static int GetId(Type mapperType)
    {
        return MapperTypes.GetOrAdd(mapperType, _ => Interlocked.Increment(ref _nextId));
    }
}

// ReSharper disable once UnusedTypeParameter
internal static class MapperType<T>
{
    // ReSharper disable once StaticMemberInGenericType
    public static readonly int Id = MapperType.GetId(typeof(T));
}