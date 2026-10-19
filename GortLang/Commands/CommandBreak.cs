using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command('!')]
internal class CommandBreak : CommandBase
{
    private IRunner? _runner;

    internal CommandBreak(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

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
            _runner = Tools.GetRunner(line, Environment.Functions);
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
