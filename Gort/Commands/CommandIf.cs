using PaleyExpressions.Runners;

namespace Gort.Commands;

[Command('{')]
internal class CommandIf(int lineNumber) : CommandBase(lineNumber)
{
    private IRunner? _runner;

    internal override void Compile(string line)
    {
        _runner = Tools.GetRunner(line);

        Environment.Stack.Push(this);
    }

    internal override CommandBase? Execute()
    {
        var result = _runner?.Interpret(Environment.Variables);

        if (result is bool b)
        {
            return b ? Next : Jump?.Next;
        }

        throw new RunTimeException($"If guard must evaluate to boolean. Line {LineNumber}");
    }

    internal CommandBase? Jump { get; set; }
}
