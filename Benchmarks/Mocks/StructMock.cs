namespace Benchmarks.Mocks;

public readonly struct StructMock
{
    public readonly object[] Array;
    public readonly bool Bool;
    public readonly int Int;
    public readonly object Obj;

    public StructMock(object[] array, bool b, int i, object obj)
    {
        Array = array;
        Bool = b;
        Int = i;
        Obj = obj;
    }
}