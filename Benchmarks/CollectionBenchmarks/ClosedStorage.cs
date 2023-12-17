using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Benchmarks.CollectionBenchmarks;

internal sealed class ClosedStorage
{
    private readonly Car[] _array;

    public ClosedStorage(IEnumerable<int> ids)
    {
        _array = ids
            .Select(id => new Car(id, Guid.NewGuid().ToString(), id))
            .ToArray();
    }

    public (int index, string name, int speed) GetValue(int id)
    {
        var array = _array;
        for (var i = 0; i < array.Length; i++)
        {
            ref readonly var element = ref array[i];
            if (element.Id != id) continue;

            return (i, element.Name, element.Speed);
        }


        return (-1, default!, default);
    }

    public ItemRef<int> TryGetItemRef(int id)
    {
        for (var i = 0; i < _array.Length; i++)
        {
            ref var element = ref _array[i];
            if (element.Id == id) return ItemRef<int>.Success(element.Name, ref element.Speed);
        }

        return ItemRef<int>.Failure();
    }

    public ItemStructRef<int> TryGetItemRefStructConstraint(int id)
    {
        for (var i = 0; i < _array.Length; i++)
        {
            ref var element = ref _array[i];
            if (element.Id == id) return ItemStructRef<int>.Success(element.Name, ref element.Speed);
        }

        return ItemStructRef<int>.Failure();
    }
    
    public bool TryGetValue(int id, out int index, out string name, out int speed)
    {
        var array = _array;
        for (var i = 0; i < array.Length; i++)
        {
            ref readonly var element = ref array[i];
            if (element.Id != id) continue;

            index = i;
            name = element.Name;
            speed = element.Speed;
            return true;
        }

        index = default;
        name = default!;
        speed = default;

        return false;
    }

    public int this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _array[index].Speed = value;
    }

    private struct Car
    {
        public readonly int Id;
        public readonly string Name;
        public int Speed;

        public Car(int id, string name, int speed)
        {
            Id = id;
            Name = name;
            Speed = speed;
        }
    }
}