using GortLang;
using System.Text;
using GortLang.Commands;

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

    [Theory]
    [InlineData("a")]
    [InlineData("_a")]
    [InlineData("_")]
    [InlineData("z1234")]
    [InlineData("X0_9")]
    [InlineData("__")]
    public void ValidIdentifiers_Pass(string text)
    {
        Assert.True(Tools.IsIdentifier(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("9")]
    [InlineData("-")]
    [InlineData("9z1234")]
    [InlineData("#0_9")]
    [InlineData("+k")]
    public void InvalidIdentifiers_DoNotPass(string text)
    {
        Assert.False(Tools.IsIdentifier(text));
    }

    [Theory]
    [InlineData(typeof(CommandIf), '{')]
    [InlineData(typeof(CommandLoop), '[')]
    [InlineData(typeof(CommandEndif), '}')]
    [InlineData(typeof(CommandEndLoop), ']')]
    [InlineData(typeof(CommandBreak), '!')]
    [InlineData(typeof(CommandDisplay), ':')]
    [InlineData(typeof(CommandAssign), '$')]
    public void GetSigil_ReturnsCorrectSigil(Type commandType, char expectedSigil)
    {
        var actualSigil = Tools.GetSigil(commandType);
        Assert.Equal(expectedSigil, actualSigil);
    }

    [Fact]
    public void GetSigil_ReturnsXIfNoSigil()
    {
        var actualSigil = Tools.GetSigil(typeof(CommandElse));
        Assert.Equal('X', actualSigil);
    }
}