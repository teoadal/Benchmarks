using System;

namespace Benchmarks.SqlBenchmarks;

internal sealed class MockDbo
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public bool IsDeleted { get; set; }
}