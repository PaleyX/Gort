using PaleyExpressions.Runners;

namespace GortLang.Commands;

[Command('{')]
internal sealed class CommandIf : CommandBase
{
    private IRunner? _runner;

    internal CommandIf(Environment environment, int lineNumber) : base(environment, lineNumber)
    {
    }

    internal override void Compile(string line)
    {
        _runner = Tools.GetRunner(line, Environment.Functions);

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
