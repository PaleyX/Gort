using System.Text;
using GortLang;

namespace UnitTests;

public class CommandTests
{
    private Dictionary<string, object?> _variables = new();

    [Fact]
    public void CommandIf_FalseJumpsToEndif()
    {
        var source = """
                     z 10
                     { 1 == 2
                       z 99
                     }
                     """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(10d, _variables["z"]);
    }

    [Fact]
    public void CommandIf_TrueJumpsToNext()
    {
        var source = """
                     z 10
                     { 1 == 1
                       z 99
                     }
                     """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(99d, _variables["z"]);
    }

    [Fact]
    public void CommandIf_WithElseJumpsToElse()
    {
        var source = """
                     z 10
                     { 1 == 2
                       z 99
                     }
                     {
                       z 55
                     }
                     """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(55d, _variables["z"]);
    }

    [Fact]
    public void CommandBreaK_WithNoClauseExitsLoop()
    {
        var source = """
                     n 0
                     [
                       !
                       n n+1
                     ]
                     """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(0d, _variables["n"]);
    }

    [Fact]
    public void CommandBreaK_WithClauseExitsLoop()
    {
        var source = """
                     n 0
                     [
                       ! n == 6
                       n n+1
                     ]
                     """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(6d, _variables["n"]);
    }
}