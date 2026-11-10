using GortLang;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class ReturnValueTests
{
    private readonly Dictionary<string, object?> _variables = new();

    [Fact]
    public void AssignSequence_ReturnsCorrectly()
    {
        const string source = """
                              a 10
                              b 20.89
                              c a+b
                              d c*1_000_000
                              tf true
                              s "Hello"
                              """;

        var runner = new Interpreter(source, _variables, null);
        var result = runner.Run();

        Assert.Equal("Hello", result);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void IfElse_ReturnsCorrect(bool value, bool expected)
    {
        _variables.Clear();
        _variables["a"] = value;

        var source = """
                     { a 
                        _ !a
                     }
                     {
                        _ !a
                     }
                     """;

        var runner = new Interpreter(source, _variables, null);
        var result = runner.Run();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Loop_ReturnsCorrectly()
    {
        var source = """
                     n 0
                     [ n < 10
                        n n + 1
                     ]
                     """;

        var runner = new Interpreter(source, _variables, null);
        var result = runner.Run();

        Assert.Equal(false, result);
    }

    [Fact]
    public void Display_ReturnsCorrectly()
    {
        var source = """
                     : "Hello"
                     : " "
                     : "World"
                     """;
        var runner = new Interpreter(source, _variables, null);
        var result = runner.Run();

        Assert.Equal("World", result);
    }

    [Fact]
    public void Break_ReturnsCorrectly()
    {
        var source = """
                     n 0
                     [ n < 10
                        n n + 1
                        ! n > 5
                     ]
                     """;
        var runner = new Interpreter(source, _variables, null);
        var result = runner.Run();

        Assert.Equal(true, result);
    }
}