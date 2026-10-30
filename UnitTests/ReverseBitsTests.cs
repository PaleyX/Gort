using GortLang;

namespace UnitTests;

public class ReverseBitsTests
{
    [Theory]
    [InlineData(0x00000000)] // All bits are zero; verifies the zero result.
    [InlineData(0x00000001)] // Only the least-significant bit is set.
    [InlineData(0x00000002)] // Tests a bit that moves one position farther.
    [InlineData(0x00000004)] // Tests a power of two away from either boundary.
    [InlineData(0x80000000)] // Only the most-significant 32-bit is set.
    [InlineData(0xFFFFFFFF)] // All 32 bits are set.
    [InlineData(0x55555555)] // Alternating zero and one bits, starting with zero.
    [InlineData(0xAAAAAAAA)] // Alternating zero and one bits, starting with one.
    [InlineData(0x0000FFFF)] // The lower 16 bits are set.
    [InlineData(0xFFFF0000)] // The upper 16 bits are set.
    [InlineData(0x000000FF)] // A complete lower byte is set.
    [InlineData(0xFF000000)] // A complete upper byte is set.
    [InlineData(0x12345678)] // Mixed bits across all four bytes.
    [InlineData(0x87654321)] // Another mixed pattern with reversed byte ordering.
    [InlineData(0x000FFFFD)] // Existing case with sparse lower bits.
    public void ReverseBitsWorks(double value)
    {
        var source = """
                     r 0
                     i 0
                     [ i < 32
                         r r << 1
                         r r | (n & 1)
                         n n >> 1

                     	i i+1
                     ]
                     """;

        var variables = new Dictionary<string, object?>
        {
            { "n", value }
        };

        // run the program
        var runner = new Interpreter(source, variables, null);
        runner.Run();

        // C# Reverse bits 
        var expected = ReverseBits(value);
        
        Assert.Equal(expected, variables["r"]);
    }

    private static double ReverseBits(double n)
    {
        ulong result = 0;
        var b = (ulong)n;

        for (var i = 0; i < 32; i++)
        {
            result = (result << 1) | (b & 1);
            b >>= 1;
        }

        return result;
    }
}