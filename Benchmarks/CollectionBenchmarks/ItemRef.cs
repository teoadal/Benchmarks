using System.Runtime.CompilerServices;
using Benchmarks.Utils;

namespace Benchmarks.CollectionBenchmarks;

public readonly ref struct ItemRef<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemRef<T> Success(string name, ref T value) => new(name, ref value, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemRef<T> Failure() => new(string.Empty, ref _emptyValue, false);

    private static T _emptyValue = default!;

    public readonly string Name;

    public readonly bool Succeeded;

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (!Succeeded) Errors.EmptyReference();
            return ref _value;
        }
    }

    private readonly ref T _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ItemRef(string name, ref T value, bool succeeded)
    {
        Name = name;
        Succeeded = succeeded;
        _value = ref value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(in ItemRef<T> actor) => actor.Succeeded;
}

public readonly ref struct ItemStructRef<T>
    where T: struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemStructRef<T> Success(string name, ref T value) => new(name, ref value, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemStructRef<T> Failure() => new(string.Empty, ref _emptyValue, false);

    private static T _emptyValue = default!;

    public readonly string Name;

    public readonly bool Succeeded;

    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (!Succeeded) Errors.EmptyReference();
            return ref _value;
        }
    }

    private readonly ref T _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ItemStructRef(string name, ref T value, bool succeeded)
    {
        Name = name;
        Succeeded = succeeded;
        _value = ref value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(in ItemStructRef<T> actor) => actor.Succeeded;
}