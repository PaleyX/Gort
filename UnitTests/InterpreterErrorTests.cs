using System.Text;
using GortLang;

namespace UnitTests;

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
        var output = new StringBuilder();
        var runner = new Interpreter("?", null, null, text => output.Append(text));

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error: Unknown command '?' - line 1{System.Environment.NewLine}", output.ToString());
    }

    [Fact]
    public void AssignmentWithoutExpression_ReportsCompileError()
    {
        var output = new StringBuilder();
        var runner = new Interpreter("$value", null, null, text => output.Append(text));

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Bad assign - line 1{System.Environment.NewLine}", output.ToString());
    }

    [Fact]
    public void InvalidAssignmentIdentifier_ReportsCompileError()
    {
        var output = new StringBuilder();
        var runner = new Interpreter("$9value 1", null, null, text => output.Append(text));

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Invalid variable name '9value' - line 1{System.Environment.NewLine}", output.ToString());
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
        var output = new StringBuilder();
        var runner = new Interpreter("!", null, null, text => output.Append(text));

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Break outside of loop - line 1{System.Environment.NewLine}", output.ToString());
    }

    [Fact]
    public void EndloopWithText_ReportsCompileError()
    {
        var output = new StringBuilder();
        var runner = new Interpreter("] text", null, null, text => output.Append(text));

        var exception = Assert.Throws<CompileTimeException>(runner.Run);

        Assert.Equal("Compilation failed with 1 errors.", exception.Message);
        Assert.Equal($"Error compiling program: Superfluous text on endloop - line 1{System.Environment.NewLine}", output.ToString());
    }

    [Fact]
    public void EmptyProgram_CompletesSuccessfully()
    {
        var output = new StringBuilder();
        var runner = new Interpreter("\n # comment\n", null, null, text => output.Append(text));

        runner.Run();

        Assert.Empty(output.ToString());
    }

    [Fact]
    public void AssertFunction_IsWrappedWithSourceLine()
    {
        var runner = new Interpreter(": assert(false, \"failed\")", null, typeof(Gort.Functions));

        var exception = Assert.Throws<RunTimeException>(runner.Run);

        Assert.Equal("Assert on line 1: failed", exception.Message);
    }
}
