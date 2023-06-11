using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Benchmarks.Mapping;

internal sealed class ReflectionMapper<TIn, TOut> : IMapper<TIn, TOut>
{
    private Func<TIn, TOut>? _mapper;

    public ReflectionMapper()
    {
        _mapper = null;
    }

    public TOut Map(IMapper mapper, TIn input)
    {
        _mapper ??= CreateMapper(mapper);
        return _mapper(input);
    }

    private static Func<TIn, TOut> CreateMapper(IMapper mapper)
    {
        var inType = typeof(TIn);
        var outType = typeof(TOut);

        var inputProperties = inType.GetProperties().ToDictionary(property => property.Name);
        var outProperties = outType.GetProperties();

        var actions = new List<Expression>(outProperties.Length + 2);

        var input = Expression.Parameter(typeof(TIn));
        var output = Expression.Variable(typeof(TOut), "output");
        var service = Expression.Constant(mapper);
        var serviceMethod = service.Type.GetMethod(nameof(IMapper.GetMapper))!;

        actions.Add(Expression.Assign(output, Expression.New(typeof(TOut))));
        foreach (var outProperty in outProperties)
        {
            if (!inputProperties.TryGetValue(outProperty.Name, out var inProperty)) continue;

            Expression value = Expression.Property(input, inProperty);
            var existsMapper = ResolveMapper(service, serviceMethod, value, outProperty.PropertyType);
            if (existsMapper != null) value = existsMapper;

            actions.Add(Expression.Assign(Expression.Property(output, outProperty), value));
        }

        actions.Add(output);

        var body = Expression.Block(new[] {output}, actions);
        return Expression
            .Lambda<Func<TIn, TOut>>(body, input)
            .Compile();
    }

    private static Expression? ResolveMapper(
        ConstantExpression mapper,
        MethodInfo getMapperMethod,
        Expression input,
        Type output)
    {
        var generic = new[] {input.Type, output};
        var result = getMapperMethod
            .MakeGenericMethod(generic)
            .Invoke(mapper.Value!, Array.Empty<object>());

        if (result == null) return null;

        var mapperInstance = Expression.Constant(result);
        var mapperMethod = typeof(IMapper<,>)
            .MakeGenericType(generic)
            .GetMethod(nameof(IMapper<object, object>.Map))!;

        return Expression.Call(mapperInstance, mapperMethod, input);
    }
}