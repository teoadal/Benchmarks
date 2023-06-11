using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.StringBenchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[MeanColumn, MemoryDiagnoser]
public class CompareBenchmark
{
    [Benchmark(Baseline = true)]
    public int BigString()
    {
        var i = 0;
        var result = 0;
        while (i + Search.Length < _bigString.Length)
        {
            result += string.Compare(
                _bigString, i,
                Search, 0, Search.Length,
                false, CultureInfo.InvariantCulture);

            i += Search.Length;
        }

        return result;
    }

    [Benchmark]
    public int SmallString()
    {
        var i = 0;
        var result = 0;
        while (i + Search.Length < _smallString.Length)
        {
            result += string.Compare(
                _smallString, i,
                Search, 0, Search.Length,
                false, CultureInfo.InvariantCulture);

            i += Search.Length;
        }

        return result;
    }

    private const string Search = "abc";
    private string _bigString = null!;
    private string _smallString = null!;

    [GlobalSetup]
    public void Config()
    {
        _bigString = new string(Enumerable.Range(0, 134217728).Select(i => (char) i).ToArray());
        _smallString = new string(Enumerable.Range(0, 128).Select(i => (char) i).ToArray());
    }
}