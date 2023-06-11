using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Benchmarks.Utils;

public static class StringUtils
{
    private static StringBuilder? _sharedBuilder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder GetBuilder() => Interlocked.Exchange(ref _sharedBuilder, null)
                                                ?? new StringBuilder(4096);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Flush(this StringBuilder builder)
    {
        var result = builder.ToString();
        builder.Clear();

        Interlocked.Exchange(ref _sharedBuilder, builder);
        return result;
    }
}