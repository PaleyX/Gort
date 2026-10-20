using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command('[')]
internal sealed class CommandLoop : CommandBase
{
    private IRunner? _runner;
    private List<CommandBreak>? _breaks;

    internal CommandLoop(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            _runner = Tools.GetRunner(line, Environment.Functions);
        }

        Environment.Stack.Push(this);
    }

    internal override CommandBase? Execute()
    {
        if(_runner == null)
        {
            return Next;
        }

        var result = _runner.Interpret(Environment.Variables);

        if(result is bool b)
        {
            return b ? Next : Jump?.Next;
        }

        throw new RunTimeException($"Loop guard must evaluate to boolean. Line {LineNumber}");
    }

    internal CommandBase? Jump { get; set; }

    internal void RegisterBreak(CommandBreak commandBreak)
    {
        _breaks ??= [];

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
