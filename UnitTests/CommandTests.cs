using System.Text;
using GortLang;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);
        
        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(10d, _variables["z"]);

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(99d, _variables["z"]);

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(55d, _variables["z"]);

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(0d, _variables["n"]);

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(6d, _variables["n"]);

        Console.SetOut(savedOut);
    }

    [Fact]
    public void CommandBreak_WithFalseClauseContinuesLoop()
    {
        const string source = """
                              n 0
                              [ n < 3
                                ! n < 0
                                n n+1
                              ]
                              """;

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(3d, _variables["n"]);
    }

    [Fact]
    public void CommandBreak_ExitsOnlyInnermostLoop()
    {
        const string source = """
                              outer 0
                              [ outer < 3
                                inner 0
                                [ inner < 3
                                  inner inner+1
                                  ! inner == 2
                                ]
                                outer outer+1
                              ]
                              """;

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(3d, _variables["outer"]);
        Assert.Equal(2d, _variables["inner"]);
    }

    [Fact]
    public void GuardlessLoop_IsValidAndCanBeExitedWithBreak()
    {
        const string source = """
                              n 0
                              [
                                n n+1
                                !
                              ]
                              """;

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(1d, _variables["n"]);
    }

    [Fact]
    public void CommandIf_TrueBranchSkipsElseBody()
    {
        const string source = """
                              z 10
                              { true
                                z 20
                              }
                              {
                                z 30
                              }
                              """;

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(20d, _variables["z"]);
    }

    [Fact]
    public void NestedIfBlocksSelectTheCorrectBranch()
    {
        const string source = """
                              z 0
                              { true
                                { false
                                  z 1
                                }
                                {
                                  z 2
                                }
                              }
                              """;

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(2d, _variables["z"]);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(6d, _variables["n"]);

        Console.SetOut(savedOut);
    }

    [Fact]
    public void CommandDisplay_Works()
    {
        const string source = """
                              n 10
                              : n
                              : n*n
                              :: "hello"
                              :: " "
                              :: "world"
                              """;

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        var nl = System.Environment.NewLine;

        Assert.Equal(20, sw.ToString().Length);
        Assert.Equal($"10{nl}100{nl}hello world", sw.ToString());

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);
        runner.Run();

        Assert.Equal(0, sw.ToString().Length);
        Assert.Equal(10d, _variables["a"]);
        Assert.Equal(20.89d, _variables["b"]);
        Assert.Equal(30.89d, _variables["c"]);
        Assert.Equal(30890000d, _variables["d"]);
        Assert.Equal(true, _variables["tf"]);
        Assert.Equal("Hello", _variables["s"]);

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: Loop '[' .. ']' Endloop mismatch - line 3" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void EmptyStackOnEndloop_ThrowsCorrectly()
    {
        const string source = "]";

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: Loop '[' .. ']' Endloop mismatch - line 1" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: If '{' .. '}' Endif mismatch - line 4" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void EmptyStackOnEndif_ThrowsCorrectly()
    {
        const string source = "}";

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: If '{' .. '}' Endif mismatch - line 1" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void NoEndif_ThrowsCorrectly()
    {
        const string source = "{ true";

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error: Unmatched '{' - line 1" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
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

        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter(source, _variables, null);

        var ex = Assert.Throws<CompileTimeException>(runner.Run);
        Assert.Equal("Compilation failed with 1 errors.", ex.Message);
        Assert.Equal("Error compiling program: An else: '{' must immediately follow an endif: '}' - line 3" + System.Environment.NewLine, sw.ToString());

        Console.SetOut(savedOut);
    }
}

// TODO: standardise error message format