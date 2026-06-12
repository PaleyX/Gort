using PaleyExpressions;

namespace Gort.Commands;

[Command('[')]
internal class CommandLoop(int lineNumber) : CommandBase(lineNumber)
{
    private AstRunner? _astRunner;
    private List<CommandBreak>? _breaks;

    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            _astRunner = Tools.GetAst(line);
        }

        Environment.Stack.Push(this);
    }

    internal override CommandBase? Execute()
    {
        if(_astRunner == null)
        {
            return Next;
        }

        var result = _astRunner.Interpret(Environment.Variables);

        if(result is bool b)
        {
            return b ? Next : Jump?.Next;
        }

        throw new RunTimeException($"Loop guard must evaluate to boolean. Line {LineNumber}");
    }

    internal CommandBase? Jump { get; set; }

    internal void RegisterBreak(CommandBreak commandBreak)
    {
        _breaks ??= new();

        _breaks.Add(commandBreak);
    }

    internal void SetBreaks(CommandEndLoop endLoop)
    {
        if (_breaks == null)
        {
            return;
        }

        foreach (var commandBreak in _breaks)
        {
            commandBreak.Jump = endLoop;
        }
    }
}
