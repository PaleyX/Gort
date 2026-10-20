using System.Text;
using GortLang;

namespace UnitTests;

public class CommandTests
{
    private readonly Dictionary<string, object?> _variables = new();

    [Fact]
    public void CommandIf_FalseJumpsToEndif()
    {
        const string source = """
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
        const string source = """
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
        const string source = """
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
    public void CommandBreak_WithNoClauseExitsLoop()
    {
        const string source = """
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
    public void CommandBreak_WithClauseExitsLoop()
    {
        const string source = """
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

    [Fact]
    public void CommandBreak_WithNoClauseExitsLoopWhenInsideIf()
    {
        const string source = """
                              n 0
                              [
                                { n == 6
                                  !
                                }
                                n n+1
                              ]
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(6d, _variables["n"]);
    }

    [Fact]
    public void CommandDisplay_Works()
    {
        const string source = """
                              n 10
                              : n
                              : n*n
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        // this isn't great, but it works for now.
        // The display command writes to the output stream,
        // but doesn't add a newline if output is set to a StringBuilder
        Assert.Equal(5, sb.Length);
        Assert.Equal("10100", sb.ToString());
    }

    [Fact]
    public void CommandAssign_Works()
    {
        const string source = """
                              a 10
                              b 20.89
                              c a+b
                              d c*1_000_000
                              tf true
                              s "Hello"
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));
        runner.Run();

        Assert.Equal(0, sb.Length);
        Assert.Equal(10d, _variables["a"]);
        Assert.Equal(20.89d, _variables["b"]);
        Assert.Equal(30.89d, _variables["c"]);
        Assert.Equal(30890000d, _variables["d"]);
        Assert.Equal(true, _variables["tf"]);
        Assert.Equal("Hello", _variables["s"]);
    }
}