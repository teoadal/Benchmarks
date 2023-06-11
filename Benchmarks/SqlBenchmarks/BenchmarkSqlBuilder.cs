using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Benchmarks.SqlBenchmarks.Builder1;
using Benchmarks.SqlBenchmarks.Builder2;

namespace Benchmarks.SqlBenchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[MeanColumn, MemoryDiagnoser]
public class BenchmarkSqlBuilder
{
    [Benchmark(Baseline = true)]
    public int Builder1()
    {
        var sql = Sql<MockDbo>.Select(
            select => select.Property(dbo => dbo.Id).Property(dbo => dbo.Name),
            where => where.IsFalse(dbo => dbo.IsDeleted));

        return sql.Length * DateTime.Now.Millisecond;
    }

    [Benchmark]
    public int Builder2()
    {
        var sql = SqlExpression<MockDbo>
            .Select(select => select.Property(dbo => dbo.Id).Property(dbo => dbo.Name))
            .Where(where => where.IsFalse(dbo => dbo.IsDeleted))
            .Build();

        return sql.Length * DateTime.Now.Millisecond;
    }
}