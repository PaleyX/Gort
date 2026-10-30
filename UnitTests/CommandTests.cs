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

        var nl = System.Environment.NewLine;

        Assert.Equal(9, sb.Length);
        Assert.Equal($"10{nl}100{nl}", sb.ToString());
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

    [Fact]
    public void MisplacedEndloop_ThrowsCorrectly()
    {
        const string source = """
                              [ true
                                { true
                                  ]
                                }
                              ] 
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: Loop '[' .. ']' Endloop mismatch - line 3" + System.Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void EmptyStackOnEndloop_ThrowsCorrectly()
    {
        const string source = """
                              ] 
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: Loop '[' .. ']' Endloop mismatch - line 1" + System.Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void MisplacedLoop_ThrowsCorrectly()
    {
        const string source = """
                              [ true
                                { true
                                  [
                                }
                              ] 
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: If '{' .. '}' Endif mismatch - line 4" + System.Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void EmptyStackOnEndif_ThrowsCorrectly()
    {
        const string source = """
                              }
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: If '{' .. '}' Endif mismatch - line 1" + System.Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void NoEndif_ThrowsCorrectly()
    {
        const string source = """
                              { true
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error: Unmatched '{' - line 1" + System.Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void ElseWithNoIf_ThrowsCorrectly()
    {
        const string source = """
                              [
                              ]
                              {
                              }
                              """;

        var sb = new StringBuilder();

        var runner = new Interpreter(source, _variables, null, t => sb.Append(t));

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: An else: '{' must immediately follow an endif: '}' - line 3" + System.Environment.NewLine, sb.ToString());
    }
}

// TODO: standardise error message format