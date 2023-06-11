namespace Benchmarks.Mocks;

internal sealed class ClassMock
{
    public readonly object[] Array;
    public readonly bool Bool;
    public readonly int Int;
    public readonly object Obj;

    public ClassMock(object[] array, bool b, int i, object obj)
    {
        Array = array;
        Bool = b;
        Int = i;
        Obj = obj;
    }
}