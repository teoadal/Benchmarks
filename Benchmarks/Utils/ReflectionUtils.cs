using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Benchmarks.Utils;

public static class ReflectionUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetName(Assembly assembly) => assembly.GetName().Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyInfo GetProperty(LambdaExpression expression)
    {
        return expression.Body is MemberExpression {Member: PropertyInfo propertyInfo}
            ? propertyInfo
            : throw new Exception();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetVersion(Assembly assembly) => assembly
        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
        .Version;
}