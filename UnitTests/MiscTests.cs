using GortLang;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests;

public class MiscTests
{
    private readonly Dictionary<string, object?> _variables = new();

    [Theory]
    [InlineData(0d, 0d)]
    [InlineData(1d, 1d)]
    [InlineData(2d, 1d)]
    [InlineData(3d, 2d)]
    [InlineData(7d, 3d)]
    [InlineData(0xFF, 8d)]
    [InlineData(0xFFFF, 16d)]
    public void BitCounting(double n, double expected)
    {
        var source = """
                     c 0
                     [ n > 0 
                        c c + (n & 1)
                        n n >> 1
                     ]
                     """;

        var sb = new StringBuilder();

        _variables["n"] = n;

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(expected, _variables["c"]);
    }
}