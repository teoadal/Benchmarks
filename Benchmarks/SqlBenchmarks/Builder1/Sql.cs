using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Benchmarks.Utils;

namespace Benchmarks.SqlBenchmarks.Builder1;

public static class Sql<T>
    where T : class
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ConcurrentDictionary<string, string> Expressions = new();

    public static string Check(Action<WhereBuilder> where, [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static (_, action) =>
        {
            var stringBuilder = StringUtils
                .GetBuilder()
                .Append("SELECT 1 FROM \"")
                .AppendTableName(typeof(T))
                .Append("\" WHERE ");

            action(new WhereBuilder(stringBuilder));
            return stringBuilder.Flush();
        }, where);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetExpression(string alias) => Expressions[alias];

    public static string Delete(Action<WhereBuilder> where, [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static (_, action) =>
        {
            var stringBuilder = StringUtils
                .GetBuilder()
                .Append("DELETE FROM \"")
                .AppendTableName(typeof(T))
                .Append("\" WHERE ");

            action(new WhereBuilder(stringBuilder));
            return stringBuilder.Flush();
        }, where);
    }

    public static string Insert([CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static _ =>
        {
            var builder = StringUtils.GetBuilder();

            var type = typeof(T);
            var properties = type.GetProperties();

            builder
                .Append("INSERT INTO \"")
                .AppendTableName(type)
                .Append("\" (");

            var first = true;
            foreach (var property in properties)
            {
                if (first) first = false;
                else builder.Append(',');

                builder
                    .Append('"')
                    .AppendColumnName(property)
                    .Append('"');
            }

            builder.Append(") VALUES (");

            first = true;
            foreach (var property in properties)
            {
                if (first) first = false;
                else builder.Append(',');

                builder
                    .Append('@')
                    .AppendColumnName(property);
            }

            return builder
                .Append(')')
                .Flush();
        });
    }

    public static string Select(Action<WhereBuilder>? where = null, [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static (_, action) =>
        {
            var stringBuilder = StringUtils
                .GetBuilder()
                .Append("SELECT * FROM \"")
                .AppendTableName(typeof(T))
                .Append('"');

            // ReSharper disable once InvertIf
            if (action != null)
            {
                stringBuilder.Append(" WHERE ");
                action.Invoke(new WhereBuilder(stringBuilder));
            }

            return stringBuilder.Flush();
        }, where);
    }

    public static string Select(
        Action<SelectBuilder> select,
        Action<WhereBuilder>? where = null,
        [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static (_, action) =>
        {
            var stringBuilder = StringUtils
                .GetBuilder()
                .Append("SELECT ");

            action.Select(new SelectBuilder(stringBuilder));

            stringBuilder
                .Append(" FROM \"")
                .AppendTableName(typeof(T))
                .Append('"');

            var whereAction = action.Where;

            // ReSharper disable once InvertIf
            if (whereAction != null)
            {
                stringBuilder.Append(" WHERE ");
                whereAction.Invoke(new WhereBuilder(stringBuilder));
            }

            return stringBuilder.Flush();
        }, (Select: select, Where: where));
    }

    public static string Update(
        Action<UpdateBuilder> update,
        Action<WhereBuilder>? where = null,
        [CallerMemberName] string? alias = null)
    {
        if (string.IsNullOrEmpty(alias)) Errors.AliasNullOrEmpty();

        return Expressions.GetOrAdd(alias!, static (_, actions) =>
        {
            var stringBuilder = StringUtils
                .GetBuilder()
                .Append("UPDATE \"")
                .AppendTableName(typeof(T))
                .Append("\" ");

            actions.Update(new UpdateBuilder(stringBuilder));

            var whereAction = actions.Where;

            // ReSharper disable once InvertIf
            if (whereAction != null)
            {
                stringBuilder.Append(" WHERE ");
                whereAction(new WhereBuilder(stringBuilder));
            }

            return stringBuilder.Flush();
        }, (Update: update, Where: where));
    }

    public sealed class SelectBuilder
    {
        private bool _addSeparator;
        private readonly StringBuilder _builder;

        public SelectBuilder(StringBuilder builder)
        {
            _addSeparator = false;
            _builder = builder;
        }

        public SelectBuilder Property(string property)
        {
            _builder
                .Append('"')
                .Append(property)
                .Append('"');

            return this;
        }

        public SelectBuilder Property<TValue>(Expression<Func<T, TValue>> property)
        {
            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append('"');

            return this;
        }

        public SelectBuilder Raw(string rawSql)
        {
            Prepare();

            _builder.Append(rawSql);
            return this;
        }

        private void Prepare()
        {
            if (_addSeparator) _builder.Append(',');
            _addSeparator = true;
        }
    }

    public sealed class UpdateBuilder
    {
        private bool _addSeparator;
        private readonly StringBuilder _builder;

        public UpdateBuilder(StringBuilder builder)
        {
            _addSeparator = false;
            _builder = builder;
        }

        public UpdateBuilder Raw(string rawSql)
        {
            Prepare();

            _builder.Append(rawSql);
            return this;
        }

        public UpdateBuilder SetFalse(string property)
        {
            Prepare();

            _builder
                .Append("SET \"")
                .Append(property)
                .Append("\" = false");

            return this;
        }

        public UpdateBuilder SetFalse(Expression<Func<T, bool>> property)
        {
            Prepare();

            _builder
                .Append("SET \"")
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" = false");

            return this;
        }

        public UpdateBuilder SetTrue(Expression<Func<T, bool>> property)
        {
            Prepare();

            _builder
                .Append("SET \"")
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" = true");

            return this;
        }

        public UpdateBuilder SetValue<TValue>(Expression<Func<T, TValue>> property, string? payloadAlias = null)
        {
            Prepare();

            var propertyInfo = ReflectionUtils.GetProperty(property);

            _builder
                .Append("SET \"")
                .AppendColumnName(propertyInfo)
                .Append("\" = ");

            if (string.IsNullOrEmpty(payloadAlias)) _builder.AppendColumnAlias(propertyInfo);
            else _builder.Append(payloadAlias);

            return this;
        }

        private void Prepare()
        {
            if (_addSeparator) _builder.Append(',');
            _addSeparator = true;
        }
    }

    public sealed class WhereBuilder
    {
        private bool _addSeparator;
        private readonly StringBuilder _builder;

        public WhereBuilder(StringBuilder builder)
        {
            _addSeparator = false;
            _builder = builder;
        }

        public WhereBuilder IsFalse(Expression<Func<T, bool>> property)
        {
            Prepare();

            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" = false");

            return this;
        }

        public WhereBuilder IsEqual<TValue>(Expression<Func<T, TValue>> property, string? payloadAlias = null)
        {
            Prepare();

            var propertyInfo = ReflectionUtils.GetProperty(property);

            _builder
                .Append('"')
                .AppendColumnName(propertyInfo)
                .Append("\" = ");

            if (string.IsNullOrEmpty(payloadAlias)) _builder.AppendColumnAlias(propertyInfo);
            else
            {
                _builder
                    .Append('@')
                    .Append(payloadAlias);
            }

            return this;
        }

        public WhereBuilder IsEqual(string property, string? payloadAlias = null)
        {
            Prepare();

            _builder
                .Append('"')
                .Append(property)
                .Append("\" = ")
                .Append('@')
                .Append(payloadAlias ?? property);

            return this;
        }

        public WhereBuilder IsNull<TValue>(Expression<Func<T, TValue?>> property)
        {
            Prepare();

            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" IS NULL");

            return this;
        }

        public WhereBuilder IsTrue(Expression<Func<T, bool>> property)
        {
            Prepare();

            _builder
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append("\" = true");

            return this;
        }

        public WhereBuilder IsTrue(string property)
        {
            Prepare();

            _builder
                .Append('"')
                .Append(property)
                .Append("\" = true");

            return this;
        }

        public WhereBuilder Raw(string raw)
        {
            Prepare();

            _builder.Append(raw);
            return this;
        }

        public WhereBuilder Returning<TValue>(Expression<Func<T, TValue>> property)
        {
            _builder
                .Append(" RETURNING ")
                .Append('"')
                .AppendColumnName(ReflectionUtils.GetProperty(property))
                .Append('"');

            return this;
        }

        private void Prepare()
        {
            if (_addSeparator) _builder.Append(" AND ");
            _addSeparator = true;
        }
    }
}