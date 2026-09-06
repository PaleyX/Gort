using PaleyExpressions.Runners;

namespace Gort.Commands;

[Command('!')]
internal class CommandBreak(int lineNumber) : CommandBase(lineNumber)
{
    private IRunner? _runner;

    internal override void Compile(string line)
    {
        var loop = Environment.FindMostRecentInStack<CommandLoop>();
        if(loop == null)
        {
            throw new CompileTimeException("Break outside of loop");
        }

        loop.RegisterBreak(this);

        if (!string.IsNullOrWhiteSpace(line))
        {
            _runner = Tools.GetRunner(line);
        }
    }

    internal override CommandBase? Execute()
    {
        if (_runner == null)
        {
            return Jump.Next;
        }

        var result = _runner.Interpret(Environment.Variables);

        if (result is bool b)
        {
            return b ? Jump.Next : Next;
        }

        throw new RunTimeException($"Break guard must evaluate to boolean. Line {LineNumber}");
    }

    internal CommandBase Jump { get; set; } = null!;
}
