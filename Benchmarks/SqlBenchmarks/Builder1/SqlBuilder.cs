using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Benchmarks.Utils;

namespace Benchmarks.SqlBenchmarks.Builder1;

internal static class SqlBuilder
{
    // https://www.npgsql.org/doc/types/basic.html
    private static readonly Dictionary<Type, Func<PropertyInfo, string>> SqlTypes = new()
    {
        {typeof(Guid), static _ => "uuid"},
        {typeof(DateTime), static _ => "timestamp"},
        {typeof(TimeSpan), static _ => "time"},
        {typeof(short), static _ => "smallint"},
        {typeof(int), static _ => "integer"},
        {typeof(long), static _ => "bigint"},
        {typeof(float), static _ => "real"},
        {typeof(double), static _ => "double precision"},
        {typeof(decimal), static _ => "numeric"},
        {typeof(bool), static _ => "boolean"},
        {typeof(byte[]), static _ => "bytea"},
        {typeof(string), StringColumType}
    };

    public static StringBuilder AppendColumnAlias(this StringBuilder builder, PropertyInfo property) => builder
        .Append('@')
        .Append(property.Name);

    public static StringBuilder AppendColumnName(this StringBuilder builder, PropertyInfo property) => builder
        .Append(property.Name);

    public static void AppendColumnType(this StringBuilder builder, PropertyInfo property)
    {
        var propertyType = property.PropertyType;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            propertyType = propertyType.GetGenericArguments()[0];
        }

        if (propertyType.IsEnum)
        {
            builder.Append("smallint");
            return;
        }

        if (!SqlTypes.TryGetValue(propertyType, out var typeBuilder))
        {
            throw new NotSupportedException($"Type {propertyType} is not supported");
        }

        builder.Append(typeBuilder(property));
    }

    public static StringBuilder AppendTableName(this StringBuilder builder, Type type)
    {
        builder
            .Append(type.Name)
            .Replace("Dbo", string.Empty);

        var lastIndex = builder.Length - 1;
        if (builder[lastIndex] == 'y')
        {
            builder
                .Remove(lastIndex, 1)
                .Append("ies");
        }
        else builder.Append('s');

        return builder;
    }

    private static string StringColumType(PropertyInfo property)
    {
        var lengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
        if (lengthAttribute == null) return "text";

        var builder = new CharBuilder(stackalloc char[128]);
        builder.Append("varchar(");
        builder.Append(lengthAttribute.Length);
        builder.Append(')');
        return builder.Flush();
    }
}