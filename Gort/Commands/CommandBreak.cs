using PaleyExpressions;

namespace Gort.Commands;

[Command('!')]
internal class CommandBreak(int lineNumber) : CommandBase(lineNumber)
{
    private AstRunner? _astRunner;

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
            _astRunner = Tools.GetAst(line);
        }
    }

    internal override CommandBase? Execute()
    {
        if (_astRunner == null)
        {
            return Jump.Next;
        }

        var result = _astRunner.Interpret(Environment.Variables);

        if (result is bool b)
        {
            return b ? Jump.Next : Next;
        }

        throw new RunTimeException($"Break guard must evaluate to boolean. Line {LineNumber}");
    }

    internal CommandBase Jump { get; set; } = null!;
}
