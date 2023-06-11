namespace Benchmarks.Mapping;

public interface IMapper
{
    IMapper<TIn, TOut>? GetMapper<TIn, TOut>();

    TOut Map<TIn, TOut>(TIn input);
}

public interface IMapper<in TIn, out TOut> : ITypeMapper
{
    TOut Map(IMapper mapper, TIn input);
}