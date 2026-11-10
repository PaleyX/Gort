using GortLang;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class InterpreterErrorTests
{
    [Fact]
    public void NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Interpreter(null, null, null));
    }

    [Fact]
    public void UnknownCommand_ReportsCompileError()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("?", null, null);

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error: Unknown command '?' - line 1{System.Environment.NewLine}", sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void AssignmentWithoutExpression_ReportsCompileError()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("$value", null, null);

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Bad assign - line 1{System.Environment.NewLine}", sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void InvalidAssignmentIdentifier_ReportsCompileError()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("$9value 1", null, null);

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Invalid variable name '9value' - line 1{System.Environment.NewLine}", sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void IfGuardMustBeBoolean()
    {
        var runner = new Interpreter($"{{ 1{System.Environment.NewLine}}}", null, null);

        var exception = Assert.Throws<RunTimeException>(runner.Run);

        Assert.Equal("If guard must evaluate to boolean. Line 1", exception.Message);
    }

    [Fact]
    public void LoopGuardMustBeBoolean()
    {
        var runner = new Interpreter($"[ 1{System.Environment.NewLine}]", null, null);

        var exception = Assert.Throws<RunTimeException>(runner.Run);

        Assert.Equal("Loop guard must evaluate to boolean. Line 1", exception.Message);
    }

    [Fact]
    public void ConditionalBreakGuardMustBeBoolean()
    {
        var runner = new Interpreter($"[ true{System.Environment.NewLine}! 1{System.Environment.NewLine}]", null, null);

        var exception = Assert.Throws<RunTimeException>(runner.Run);

        Assert.Equal("Break guard must evaluate to boolean. Line 2", exception.Message);
    }

    [Fact]
    public void BreakOutsideLoop_ReportsCompileError()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("!", null, null);

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Break outside of loop - line 1{System.Environment.NewLine}", sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void EndloopWithText_ReportsCompileError()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("] text", null, null);

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Superfluous text on endloop - line 1{System.Environment.NewLine}", sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void EmptyProgram_CompletesSuccessfully()
    {
        using var sw = new StringWriter();
        var savedOut = Console.Out;
        Console.SetOut(sw);

        var runner = new Interpreter("\n # comment\n", null, null);

        runner.Run();

        Assert.Empty(sw.ToString());

        Console.SetOut(savedOut);
    }

    [Fact]
    public void AssertFunction_IsWrappedWithSourceLine()
    {
        var runner = new Interpreter(": assert(false, \"failed\")", null, typeof(Gort.Functions));

        var exception = Assert.Throws<RunTimeException>(runner.Run);

        Assert.Equal("Assert on line 1: failed", exception.Message);
    }
}
