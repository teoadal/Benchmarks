using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Benchmarks;

internal static class CollectionUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Cut<T>(T[] array, int index)
    {
        var length = array.Length - 1;

        if (index < length)
        {
            Array.Copy(array, index + 1, array, index, length - index);
        }

        array[length] = default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureCapacity<T>(ref T[] array, int capacity, bool clear = false)
    {
        if (array.Length < capacity) Resize(ref array, capacity, clear);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Resize<T>(ref T[] array, int? newLength = null, bool clear = false)
    {
        var newArray = new T[newLength ?? array.Length * 2];

        if (array.Length > 0)
        {
            Array.Copy(array, newArray, array.Length);
            if (clear) Array.Clear(array, 0, array.Length);
        }

        array = newArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Resize<T>(ref T[] array, ArrayPool<T> pool, int newLength, bool clear = false)
    {
        var newArray = pool.Rent(newLength);

        if (array.Length > 0)
        {
            Array.Copy(array, newArray, array.Length);
            pool.Return(array, clear);
        }

        array = newArray;
    }

    public static T[] Shuffle<T>(T[] array, Random random)
    {
        var length = array.Length;

        var result = new T[length];
        Array.Copy(array, result, length);

        var n = length;
        while (n > 1)
        {
            var k = random.Next(n--);
            (result[n], result[k]) = (result[k], result[n]);
        }

        return result;
    }
}