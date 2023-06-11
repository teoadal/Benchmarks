using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Benchmarks.SqlBenchmarks.Builder1;
using Benchmarks.Utils;

namespace Benchmarks.SqlBenchmarks.Builder2;

internal readonly struct SqlExpression<T>
    where T : class
{
    private static readonly ConcurrentDictionary<string, SqlExpression<T>> Expressions = new();

    private readonly string? _expression;
    private readonly string _alias;
    private readonly StringBuilder? _builder;

    private SqlExpression(string alias, StringBuilder? builder, string? expression)
    {
        _alias = alias;
        _builder = builder;
        _expression = expression;
    }

    public static SqlExpression<T> Select([CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();
        if (Expressions.TryGetValue(alias!, out var exists)) return exists;

        var builder = StringUtils.GetBuilder();

        builder
            .Append("SELECT * FROM \"")
            .AppendTableName(typeof(T))
            .Append('"');

        return new SqlExpression<T>(alias!, builder, null);
    }

    public static SqlExpression<T> Select(Action<SelectBuilder> select, [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();
        if (Expressions.TryGetValue(alias!, out var exists)) return exists;

        var builder = StringUtils.GetBuilder();

        builder.Append("SELECT ");
        select(new SelectBuilder(builder, false));

        builder
            .Append(" FROM \"")
            .AppendTableName(typeof(T))
            .Append('"');

        return new SqlExpression<T>(alias!, builder, null);
    }

    public string Build()
    {
        if (_expression != null) return _expression;

        var expression = _builder!.Flush();
        if (!Expressions.TryAdd(_alias, new SqlExpression<T>(_alias, null, expression)))
        {
            Errors.AliasNullOrEmpty();
        }

        return expression;
    }

    public override string ToString() => _expression ?? _builder?.ToString() ?? string.Empty;

    public SqlExpression<T> Where(Action<WhereBuilder> where)
    {
        if (_builder != null)
        {
            where(new WhereBuilder(_builder, false));
        }

        return new SqlExpression<T>(_alias, _builder, _expression);
    }

    public readonly struct SelectBuilder
    {
        private readonly bool _addSeparator;
        private readonly StringBuilder _builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectBuilder(StringBuilder builder, bool addSeparator)
        {
            _addSeparator = addSeparator;
            _builder = builder;
        }

        public SelectBuilder Property<TValue>(Expression<Func<T, TValue>> property)
        {
            if (_addSeparator) _builder.Append(',');

            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append('"');

            return new SelectBuilder(_builder, true);
        }
    }

    public readonly struct WhereBuilder
    {
        private readonly bool _addSeparator;
        private readonly StringBuilder _builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WhereBuilder(StringBuilder builder, bool addSeparator)
        {
            _addSeparator = addSeparator;
            _builder = builder;
        }

        public WhereBuilder IsFalse(Expression<Func<T, bool>> property)
        {
            if (_addSeparator) _builder.Append(" AND ");

            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" = false");

            return new WhereBuilder(_builder, true);
        }
    }
}