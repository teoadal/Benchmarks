using System;

namespace Benchmarks.Mapping;

internal static class MapperErrors
{
    public static void MapperAlreadyExists(ITypeMapper mapper)
    {
        var type = mapper.GetType();
        if (type.IsGenericType)
        {
            var arguments = type.GetGenericArguments();

            throw new Exception(arguments.Length == 2
                ? $"Mapper from '{arguments[0]}' to {arguments[1]} already exists"
                : $"Mapper from ({arguments[0]}, {arguments[1]}) to {arguments[2]} already exists");
        }

        throw new Exception($"Mapper {type} already exists");
    }

    public static void MapperNotFound(Type input, Type output)
    {
        throw new Exception($"Mapper from {input} to {output} isn't found");
    }
}